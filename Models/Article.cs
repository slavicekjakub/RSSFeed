namespace RSSFeed.Models
{
  public class Article
  {
    public string Title { get; set; } = String.Empty;
    public string Link { get; set; } = String.Empty;
    public string? Description { get; set; }
    public DateTime? PublishDate { get; set; }
    public string? Author { get; set; }
  }
}
