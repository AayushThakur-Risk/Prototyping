namespace MyBlazorWasmApp.Models
{
    public class UrlData
    {
        public string Url { get; set; }
        public bool IsHealthy { get; set; }
        public double PercentageChange { get; set; }
        public double SignificanceChange { get; set; }
        public DateTime LastScraped { get; set; }
        public List<SectionData> Sections { get; set; }
        public bool IsExpanded { get; set; } = false; // default collapsed
    }
}
