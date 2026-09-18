using marcc.Models;
using System.Text;

namespace marcc.Formats
{
    public class Parser
    {
        public MarcRecord Parse(byte[] raw, int recordNumber = 1)
        {
            if (raw == null || raw.Length == 0)
                throw new InvalidDataException($"Registro {recordNumber}: conteúdo vazio.");

            var data = RemoveTrailingRecordTerminator(raw);

            if (data.Length < Configuration.LeaderLength)
                throw new InvalidDataException($"Registro {recordNumber}: Leader menor que 24 bytes.");

            var leader = data[..Configuration.LeaderLength];

            ValidateLeader(leader, recordNumber);

            var recordLength = ParseNumber(leader, 0, 5, $"Registro {recordNumber}: tamanho do registro inválido.");

            if (recordLength != data.Length + 1)
                throw new InvalidDataException($"Registro {recordNumber}: o tamanho indicado pelo Leader é {recordLength} bytes, mas foram encontrados {data.Length + 1} bytes.");

            var baseAddress = ParseNumber(leader, 12, 5, $"Registro {recordNumber}: endereço-base inválido.");

            if (baseAddress < 25 || baseAddress > data.Length)
                throw new InvalidDataException($"Registro {recordNumber}: endereço-base inválido: {baseAddress}.");

            var directoryLength = baseAddress - Configuration.LeaderLength - 1;

            if (directoryLength < 0 || directoryLength % Configuration.DirectoryEntryLength != 0)
                throw new InvalidDataException($"Registro {recordNumber}: diretório inválido.");

            var directoryStart = Configuration.LeaderLength;
            var directoryEnd = directoryStart + directoryLength;

            if (directoryEnd >= data.Length || data[directoryEnd] != Configuration.FieldSeparator)
                throw new InvalidDataException($"Registro {recordNumber}: diretório não termina com 0x1E.");

            var directory = data[directoryStart..directoryEnd];
            var fieldData = data[(directoryEnd + 1)..];

            var record = new MarcRecord { Leader = leader };

            for (var i = 0; i < directory.Length; i += Configuration.DirectoryEntryLength)
            {
                var entry = directory.AsSpan(i, Configuration.DirectoryEntryLength);
                var tag = Encoding.ASCII.GetString(entry[..3]);

                ValidateTag(tag, recordNumber);

                var length = ParseNumber(entry, 3, 4, $"Registro {recordNumber}: tamanho inválido no campo {tag}.");
                var start = ParseNumber(entry, 7, 5, $"Registro {recordNumber}: posição inválida no campo {tag}.");

                if (length <= 0)
                    throw new InvalidDataException($"Registro {recordNumber}: campo {tag} possui tamanho inválido.");

                if (start < 0 || start + length > fieldData.Length)
                    throw new InvalidDataException($"Registro {recordNumber}: campo {tag} ultrapassa os dados.");

                var fieldDataBytes = fieldData[start..(start + length)];

                if (fieldDataBytes[^1] != Configuration.FieldSeparator)
                    throw new InvalidDataException($"Registro {recordNumber}: campo {tag} não termina com 0x1E.");

                record.Fields.Add(new MarcField { Tag = tag, Data = fieldDataBytes });
            }

            if (record.Fields.Count == 0)
                throw new InvalidDataException($"Registro {recordNumber}: nenhum campo MARC encontrado.");

            return record;
        }

        public MarcRecord ParseText(string text, int recordNumber = 1)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidDataException($"Registro {recordNumber}: texto vazio.");

            var lines = text.Split(["\r\n", "\n"], StringSplitOptions.None);
            string? leaderText = null;
            var fields = new List<MarcField>();

            foreach (var rawLine in lines)
            {
                if (string.IsNullOrEmpty(rawLine))
                    continue;

                var line = rawLine.TrimEnd('\r');

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (leaderText == null && TryExtractLeader(line, out var extractedLeader))
                {
                    leaderText = extractedLeader;
                    continue;
                }

                if (!LooksLikeMarcField(line))
                    throw new InvalidDataException($"Registro {recordNumber}: linha não reconhecida como campo MARC: '{line}'.");

                var field = ParseTextField(line, recordNumber);
                fields.Add(field);
            }

            if (leaderText == null)
                throw new InvalidDataException($"Registro {recordNumber}: Leader não encontrado.");

            if (fields.Count == 0)
                throw new InvalidDataException($"Registro {recordNumber}: nenhum campo MARC encontrado.");

            var leader = Encoding.UTF8.GetBytes(leaderText);

            return new MarcRecord
            {
                Leader = leader,
                Fields = fields
            };
        }

        private static MarcField ParseTextField(string line, int recordNumber)
        {
            if (line.Length < 3)
                throw new InvalidDataException($"Registro {recordNumber}: campo MARC inválido.");

            var tag = line[..3];

            ValidateTag(tag, recordNumber);

            var remainder = line[3..];

            if (IsControlField(tag))
            {
                var value = remainder.TrimStart();
                var bytes = Encoding.UTF8.GetBytes(value).ToList();

                bytes.Add(Configuration.FieldSeparator);

                return new MarcField
                {
                    Tag = tag,
                    Data = bytes.ToArray()
                };
            }

            remainder = remainder.TrimStart();

            if (remainder.Length < 2)
                throw new InvalidDataException($"Registro {recordNumber}: campo {tag} não possui indicadores.");

            var indicators = remainder[..2];

            if (!IsValidIndicator(indicators[0]) || !IsValidIndicator(indicators[1]))
                throw new InvalidDataException($"Registro {recordNumber}: indicadores inválidos no campo {tag}: '{indicators}'.");

            indicators = NormalizeIndicators(indicators);

            var subfieldText = remainder[2..];

            var data = new List<byte>
            {
                (byte)indicators[0],
                (byte)indicators[1]
            };

            ParseSubfields(subfieldText, data, recordNumber, tag);

            data.Add(Configuration.FieldSeparator);

            return new MarcField
            {
                Tag = tag,
                Data = data.ToArray()
            };
        }

        private static void ParseSubfields(string text, List<byte> output, int recordNumber, string tag)
        {
            var normalized = text.Replace('$', '|').Replace('‡', '|');
            var firstSubfield = normalized.IndexOf('|');

            if (firstSubfield < 0)
                throw new InvalidDataException($"Registro {recordNumber}: campo {tag} não possui subcampos.");

            var beforeSubfield = normalized[..firstSubfield];

            if (!string.IsNullOrWhiteSpace(beforeSubfield))
                throw new InvalidDataException($"Registro {recordNumber}: conteúdo antes do primeiro subcampo no campo {tag}.");

            var parts = normalized.Split('|');
            var subfieldCount = 0;

            for (var i = 1; i < parts.Length; i++)
            {
                var part = parts[i];

                if (string.IsNullOrEmpty(part))
                    continue;

                var code = part[0];

                if (!IsValidSubfieldCode(code))
                    throw new InvalidDataException($"Registro {recordNumber}: código de subcampo inválido '{code}' no campo {tag}.");

                output.Add(Configuration.SubfieldSeparator);
                output.Add((byte)code);

                var value = part.Length > 1 ? part[1..].TrimStart() : string.Empty;

                output.AddRange(Encoding.UTF8.GetBytes(value));

                subfieldCount++;
            }

            if (subfieldCount == 0)
                throw new InvalidDataException($"Registro {recordNumber}: campo {tag} não possui subcampos válidos.");
        }

        private static bool TryExtractLeader(string line, out string leader)
        {
            leader = string.Empty;

            if (line.StartsWith("000 "))
            {
                var value = line[4..];

                if (value.Length == 0)
                    return false;

                leader = NormalizeTextLeader(value);
                return true;
            }

            if (line.StartsWith("=LDR", StringComparison.OrdinalIgnoreCase))
            {
                var value = line[4..];

                if (value.StartsWith(" "))
                    value = value[1..];

                if (value.Length == 0)
                    return false;

                leader = NormalizeTextLeader(value);
                return true;
            }

            return false;
        }

        private static string NormalizeTextLeader(string source)
        {
            var leader = new char[24];
            Array.Fill(leader, ' ');

            var copyLength = Math.Min(source.Length, 24);
            source.CopyTo(0, leader, 0, copyLength);

            return new string(leader);
        }

        private static bool LooksLikeMarcField(string line)
        {
            if (line.Length < 3)
                return false;

            return line[..3].All(char.IsDigit);
        }

        private static bool IsControlField(string tag)
        {
            if (!int.TryParse(tag, out var value))
                return false;

            return value < 10;
        }

        private static bool IsValidIndicator(char value)
        {
            return value == '_' || value == ' ' || (value >= '0' && value <= '9');
        }

        private static string NormalizeIndicators(string indicators)
        {
            return indicators.Replace('_', ' ');
        }

        private static bool IsValidSubfieldCode(char code)
        {
            return char.IsLetterOrDigit(code) || code == '-' || code == '_' || code == '0';
        }

        private static void ValidateTag(string tag, int recordNumber)
        {
            if (tag.Length != 3 || !tag.All(char.IsDigit))
                throw new InvalidDataException($"Registro {recordNumber}: tag MARC inválida: '{tag}'.");
        }

        private static int ParseNumber(ReadOnlySpan<byte> data, int offset, int length, string errorMessage)
        {
            if (offset < 0 || offset + length > data.Length)
                throw new InvalidDataException(errorMessage);

            var value = 0;

            for (var i = 0; i < length; i++)
            {
                var c = data[offset + i];

                if (c < '0' || c > '9')
                    throw new InvalidDataException(errorMessage);

                value = value * 10 + c - '0';
            }

            return value;
        }

        private static byte[] RemoveTrailingRecordTerminator(byte[] raw)
        {
            var end = raw.Length;

            while (end > 0 && raw[end - 1] == Configuration.RecordTerminator)
                end--;

            return raw[..end];
        }

        private static void ValidateNumericRange(byte[] data, int offset, int length, string errorMessage)
        {
            if (offset < 0 || offset + length > data.Length)
                throw new InvalidDataException(errorMessage);

            for (var i = offset; i < offset + length; i++)
            {
                if (data[i] < (byte)'0' || data[i] > (byte)'9')
                    throw new InvalidDataException(errorMessage);
            }
        }

        private static void ValidateLeader(byte[] leader, int recordNumber)
        {
            if (leader == null || leader.Length != Configuration.LeaderLength)
                throw new InvalidDataException($"Registro {recordNumber}: Leader deve possuir 24 bytes.");

            ValidateNumericRange(leader, 0, 5, $"Registro {recordNumber}: tamanho do registro inválido.");

            if (leader[10] != (byte)'2')
                throw new InvalidDataException($"Registro {recordNumber}: quantidade de indicadores inválida.");

            if (leader[11] != (byte)'2')
                throw new InvalidDataException($"Registro {recordNumber}: tamanho do código de subcampo inválido.");

            ValidateNumericRange(leader, 12, 5, $"Registro {recordNumber}: endereço-base inválido.");

            if (leader[20] != (byte)'4')
                throw new InvalidDataException($"Registro {recordNumber}: tamanho do campo de tamanho deve ser 4.");

            if (leader[21] != (byte)'5')
                throw new InvalidDataException($"Registro {recordNumber}: tamanho da posição inicial deve ser 5.");
        }
    }
}