namespace MyBlazorWasmApp.Models
{
    public class SectionData
    {
        public string SectionName { get; set; } = string.Empty;
        public string PreviousValue { get; set; } = string.Empty;
        public string CurrentValue { get; set; } = string.Empty;
        public double PercentageChange { get; set; }
    }
}
