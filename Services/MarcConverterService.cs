using marcc.Formats;
using marcc.Formats.Iso2709;
using marcc.Models;
using System.Diagnostics;
using System.Text;

namespace marcc.Services
{
    public class MarcConverterService
    {
        private readonly Parser _parser;
        private readonly Iso2709Writer _iso2709Writer;

        public MarcConverterService()
        {
            _parser = new Parser();
            _iso2709Writer = new Iso2709Writer();
        }

        public ConversionResult ConvertFile(string inputFile, string outputFile)
        {
            var stopwatch = Stopwatch.StartNew();
            var recordsProcessed = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(inputFile))
                    throw new InvalidDataException("Arquivo de entrada não informado.");

                if (!File.Exists(inputFile))
                    throw new InvalidDataException($"Arquivo de entrada não encontrado: {inputFile}");

                ValidateOutputFile(outputFile);

                var source = File.ReadAllBytes(inputFile);

                if (source.Length == 0)
                    throw new InvalidDataException("Arquivo de entrada vazio.");

                var output = new List<byte>();

                if (LooksLikeIso2709(source))
                {
                    var records = SplitIso2709Records(source);

                    if (records.Count == 0)
                        throw new InvalidDataException("Nenhum registro ISO 2709 encontrado.");

                    for (var i = 0; i < records.Count; i++)
                    {
                        var record = _parser.Parse(records[i], i + 1);
                        output.AddRange(_iso2709Writer.Write(record));
                        recordsProcessed++;
                    }
                }
                else
                {
                    var text = DecodeText(source);
                    var records = SplitTextRecords(text);

                    if (records.Count == 0)
                        throw new InvalidDataException("O conteúdo não foi reconhecido como MARC.");

                    for (var i = 0; i < records.Count; i++)
                    {
                        var record = _parser.ParseText(records[i], i + 1);
                        output.AddRange(_iso2709Writer.Write(record));
                        recordsProcessed++;
                    }
                }

                if (recordsProcessed == 0)
                    throw new InvalidDataException("Nenhum registro MARC válido encontrado.");

                File.WriteAllBytes(outputFile, output.ToArray());

                return new ConversionResult
                {
                    Success = true,
                    OutputPath = outputFile,
                    RecordsProcessed = recordsProcessed,
                    Duration = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                return new ConversionResult
                {
                    Success = false,
                    OutputPath = outputFile,
                    RecordsProcessed = recordsProcessed,
                    Duration = stopwatch.Elapsed,
                    Error = ex.Message
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public ConversionResult ConvertText(string text, string outputDirectory, string? name = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var recordsProcessed = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    throw new InvalidDataException("Nenhum conteúdo informado.");

                if (string.IsNullOrWhiteSpace(outputDirectory))
                    throw new InvalidDataException("Diretório de saída não informado.");

                Directory.CreateDirectory(outputDirectory);

                name = string.IsNullOrWhiteSpace(name)
                    ? $"marc-{DateTime.Now:yyyyMMdd-HHmmss}"
                    : Path.GetFileNameWithoutExtension(name);

                var outputFile = Path.Combine(outputDirectory, $"{name}.mrc");
                var source = Encoding.UTF8.GetBytes(text);
                var output = new List<byte>();

                if (LooksLikeIso2709(source))
                {
                    var records = SplitIso2709Records(source);

                    for (var i = 0; i < records.Count; i++)
                    {
                        var record = _parser.Parse(records[i], i + 1);
                        output.AddRange(_iso2709Writer.Write(record));
                        recordsProcessed++;
                    }
                }
                else
                {
                    var records = SplitTextRecords(text);

                    if (records.Count == 0)
                        throw new InvalidDataException("O conteúdo não foi reconhecido como MARC.");

                    for (var i = 0; i < records.Count; i++)
                    {
                        var record = _parser.ParseText(records[i], i + 1);
                        output.AddRange(_iso2709Writer.Write(record));
                        recordsProcessed++;
                    }
                }

                if (recordsProcessed == 0)
                    throw new InvalidDataException("Nenhum registro MARC válido encontrado.");

                File.WriteAllBytes(outputFile, output.ToArray());

                return new ConversionResult
                {
                    Success = true,
                    OutputPath = outputFile,
                    RecordsProcessed = recordsProcessed,
                    Duration = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                return new ConversionResult
                {
                    Success = false,
                    OutputPath = null,
                    RecordsProcessed = recordsProcessed,
                    Duration = stopwatch.Elapsed,
                    Error = ex.Message
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private static bool LooksLikeIso2709(byte[] data)
        {
            if (data.Length < 24)
                return false;

            if (!data[..5].All(b => b >= '0' && b <= '9'))
                return false;

            if (!data[12..17].All(b => b >= '0' && b <= '9'))
                return false;

            var recordLength = ParseAsciiNumber(data, 0, 5);
            var baseAddress = ParseAsciiNumber(data, 12, 5);

            if (recordLength < 25)
                return false;

            if (baseAddress < 25)
                return false;

            if (baseAddress > data.Length)
                return false;

            if (data[10] != '2' || data[11] != '2')
                return false;

            var directoryTerminator = baseAddress - 1;

            if (directoryTerminator >= data.Length || data[directoryTerminator] != Configuration.FieldSeparator)
                return false;

            return true;
        }

        private static List<byte[]> SplitIso2709Records(byte[] data)
        {
            var records = new List<byte[]>();
            var offset = 0;

            while (offset < data.Length)
            {
                while (offset < data.Length && IsWhitespace(data[offset]))
                    offset++;

                if (offset >= data.Length)
                    break;

                if (data.Length - offset < 24)
                    throw new InvalidDataException($"Dados insuficientes para Leader na posição {offset}.");

                var recordLength = ParseAsciiNumber(data, offset, 5);

                if (recordLength < 25)
                    throw new InvalidDataException($"Record Length inválido na posição {offset}: {recordLength}.");

                if (offset + recordLength > data.Length)
                    throw new InvalidDataException($"Registro iniciado na posição {offset} ultrapassa o tamanho do arquivo.");

                var record = data[offset..(offset + recordLength)];

                if (record[^1] != Configuration.RecordTerminator)
                    throw new InvalidDataException($"Registro iniciado na posição {offset} não termina com 0x1D.");

                records.Add(record);
                offset += recordLength;
            }

            return records;
        }

        private static List<string> SplitTextRecords(string text)
        {
            var records = new List<string>();

            var lines = text
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n');

            var current = new List<string>();

            foreach (var rawLine in lines)
            {
                if (string.IsNullOrEmpty(rawLine))
                    continue;

                var line = rawLine.TrimEnd('\r');

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (IsLeaderLine(line) && current.Count > 0)
                {
                    records.Add(string.Join(Environment.NewLine, current));
                    current.Clear();
                }

                current.Add(line);
            }

            if (current.Count > 0)
                records.Add(string.Join(Environment.NewLine, current));

            return records;
        }

        private static bool IsLeaderLine(string line)
        {
            if (line.StartsWith("000 ") && line.Length >= 28)
            {
                var leader = line[4..];

                if (leader.Length >= 24)
                    leader = leader[..24];

                return LooksLikeLeader(leader);
            }

            if (line.StartsWith("=LDR", StringComparison.OrdinalIgnoreCase))
            {
                var leader = line[4..].TrimStart();

                if (leader.Length >= 24)
                    leader = leader[..24];

                return LooksLikeLeader(leader);
            }

            return false;
        }

        private static bool LooksLikeLeader(string leader)
        {
            if (leader.Length != 24)
                return false;

            if (!leader[..5].All(char.IsDigit))
                return false;

            return leader[10] == '2' && leader[11] == '2';
        }

        private static string DecodeText(byte[] source)
        {
            if (source.Length >= 3 && source[0] == 0xEF && source[1] == 0xBB && source[2] == 0xBF)
                return Encoding.UTF8.GetString(source);

            try
            {
                var utf8 = new UTF8Encoding(false, true);

                return utf8.GetString(source);
            }
            catch (DecoderFallbackException)
            {
                throw new InvalidDataException("O arquivo textual não está em UTF-8 válido.");
            }
        }

        private static int ParseAsciiNumber(byte[] data, int offset, int length)
        {
            var value = 0;

            for (var i = 0; i < length; i++)
            {
                var c = data[offset + i];

                if (c < '0' || c > '9')
                    throw new InvalidDataException("Número ISO 2709 inválido.");

                value = value * 10 + c - '0';
            }

            return value;
        }

        private static bool IsWhitespace(byte value)
        {
            return value == 0 || value == 9 || value == 10 || value == 13 || value == 32;
        }

        private static void ValidateOutputFile(string outputFile)
        {
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new InvalidDataException("Arquivo de saída não informado.");

            var fullPath = Path.GetFullPath(outputFile);
            var directory = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrWhiteSpace(directory))
                throw new InvalidDataException("Diretório de saída inválido.");

            Directory.CreateDirectory(directory);
        }
    }
}