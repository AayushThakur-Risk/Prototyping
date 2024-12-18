public class MenuItem
{
    public string? Link { get; set; }

    // public List<MenuItem> Children { get; set; } = new();
    public List<MenuItem>? SubMenus { get; set; } = new();

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
