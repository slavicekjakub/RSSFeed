using Microsoft.EntityFrameworkCore;
using RSSFeed.Models;

namespace RSSFeed.Data
{
  public class RSSFeedContext : DbContext
  {
    public RSSFeedContext(DbContextOptions<RSSFeedContext> options)
        : base(options)
    {
    }

    public DbSet<RssFeeds> RSSFeeds { get; set; } = null!;

  }
}
