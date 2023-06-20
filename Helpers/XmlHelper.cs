using RSSFeed.Models;
using System.Net;
using System.Xml;

namespace RSSFeed.Helpers
{
  public class XmlHelper
  {
    public List<Article> GetXmlFromUrl(string url)
    {
      try
      {
        var client = new WebClient();
        var xmlContent = client.DownloadString(url);

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlContent);

        List<Article> articles = new List<Article>();

        if (xmlDocument != null)
        {
          foreach (XmlNode node in xmlDocument.SelectNodes("/rss/channel/item"))
          {
            Article article = new Article();
            article.Title = node["title"].InnerText;
            article.Link = node["link"].InnerText;
            article.Description = node["description"].InnerText;
            try
            {
              article.PublishDate = DateTime.Parse(node["pubDate"].InnerText);
            }
            catch { }
            article.Author = node["dc:creator"].InnerText;
            articles.Add(article);
          }
        }
        return articles;
      }
      catch (Exception ex)
      {
        // Zpracování výjimky při stahování nebo parsování XML
        Console.WriteLine($"Chyba při získávání obsahu XML: {ex.Message}");
        return null;
      }
    }
  }
}
