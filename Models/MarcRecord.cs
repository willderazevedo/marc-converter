namespace marcc.Models
{
    public class MarcRecord
    {
        public byte[] Leader { get; set; } = [];
        public List<MarcField> Fields { get; set; } = [];
    }
}
