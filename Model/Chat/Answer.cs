namespace MyBlazorWasmApp.Models
{
    public class Answer
    {
        public string Content { get; set; }
        public string Metadata { get; set; }
        public List<string> Examples { get; set; } = new List<string>();
    }
}
