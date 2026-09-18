namespace marcc.Models
{
    public class ConversionResult
    {
        public bool Success { get; set; }
        public string? OutputPath { get; set; }
        public int RecordsProcessed { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Error {  get; set; }
    }
}
