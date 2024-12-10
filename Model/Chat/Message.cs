namespace MyBlazorWasmApp.Models
{
    public class Message
    {
        public string Content { get; set; }
        public bool IsUser { get; set; }
        public bool IsLoading { get; set; }
        public Answer Answer { get; set; }
        public List<Source> Sources { get; set; }
    }
}
