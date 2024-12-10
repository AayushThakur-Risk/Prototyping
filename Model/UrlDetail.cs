namespace MyBlazorWasmApp.Models
{
    public class UrlDetail
    {
        public string Url { get; set; }
        public string HealthStatus { get; set; }
        public bool IsBroken { get; set; }
        public TimeSpan TimeSinceLastScrape { get; set; }
        public Dictionary<string, bool> ContentChanges { get; set; } = new Dictionary<string, bool>();

        public UrlDetail(string url)
        {
            Url = url;
            HealthStatus = "Healthy";  // Ensure HealthStatus has a value when the object is created
        }
    }
}
