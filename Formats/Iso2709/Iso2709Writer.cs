using marcc.Models;
using System.Text;

namespace marcc.Formats.Iso2709
{
    public class Iso2709Writer
    {
        public byte[] Write(MarcRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            if (record.Leader == null || record.Leader.Length != Configuration.LeaderLength)
                throw new InvalidDataException("Leader inválido. Deve possuir 24 bytes.");

            if (record.Fields == null || record.Fields.Count == 0)
                throw new InvalidDataException("Registro não possui campos MARC.");

            var directory = new StringBuilder();
            var fieldData = new List<byte>();

            foreach (var field in record.Fields)
            {
                ValidateTag(field.Tag);

                if (field.Data == null || field.Data.Length == 0)
                    throw new InvalidDataException($"Campo {field.Tag} vazio.");

                if (field.Data[^1] != Configuration.FieldSeparator)
                    throw new InvalidDataException($"Campo {field.Tag} não termina com 0x1E.");

                var start = fieldData.Count;
                var length = field.Data.Length;

                if (length > 9999)
                    throw new InvalidDataException($"Campo {field.Tag} excede 9999 bytes.");

                if (start > 99999)
                    throw new InvalidDataException($"Posição inicial do campo {field.Tag} excede 99999.");

                directory.Append(field.Tag);
                directory.Append(length.ToString("D4"));
                directory.Append(start.ToString("D5"));

                fieldData.AddRange(field.Data);
            }

            var directoryBytes = Encoding.ASCII.GetBytes(directory.ToString());

            var completeDirectory = new List<byte>(directoryBytes.Length + 1);
            completeDirectory.AddRange(directoryBytes);
            completeDirectory.Add(Configuration.FieldSeparator);

            var baseAddress = Configuration.LeaderLength + completeDirectory.Count;
            var recordLength = baseAddress + fieldData.Count + 1;

            if (baseAddress > 99999)
                throw new InvalidDataException($"Endereço-base excede 99999: {baseAddress}.");

            if (recordLength > 99999)
                throw new InvalidDataException($"Tamanho do registro excede 99999: {recordLength}.");

            var leader = BuildLeader(record.Leader, recordLength, baseAddress);

            var result = new List<byte>(recordLength);
            result.AddRange(leader);
            result.AddRange(completeDirectory);
            result.AddRange(fieldData);
            result.Add(Configuration.RecordTerminator);

            if (result.Count != recordLength)
                throw new InvalidDataException($"Tamanho final inconsistente. Esperado: {recordLength}; obtido: {result.Count}.");

            return result.ToArray();
        }

        private static byte[] BuildLeader(byte[] sourceLeader, int recordLength, int baseAddress)
        {
            var leader = new byte[Configuration.LeaderLength];

            Array.Copy(sourceLeader, leader, Math.Min(sourceLeader.Length, leader.Length));

            WriteNumber(leader, 0, 5, recordLength);
            WriteNumber(leader, 12, 5, baseAddress);

            return leader;
        }

        private static void ValidateTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag) || tag.Length != 3 || !tag.All(char.IsDigit))
                throw new InvalidDataException($"Tag MARC inválida: '{tag}'.");
        }

        private static void WriteNumber(byte[] target, int offset, int length, int value)
        {
            if (value < 0)
                throw new InvalidDataException($"Valor numérico inválido: {value}.");

            var text = value.ToString($"D{length}");

            if (text.Length > length)
                throw new InvalidDataException($"Valor {value} não cabe em {length} posições.");

            for (var i = 0; i < length; i++)
                target[offset + i] = (byte)text[i];
        }
    }
}