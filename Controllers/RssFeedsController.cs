using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSSFeed.Data;
using RSSFeed.Helpers;
using RSSFeed.Models;

namespace RSSFeed.Controllers
{
  public class RssFeedsController : Controller
  {
    private readonly RSSFeedContext _context;
    private XmlHelper _xmlHelper;

    public RssFeedsController(RSSFeedContext context)
    {
      _xmlHelper = new XmlHelper();
      _context = context;
    }

    // GET: RssFeeds
    public async Task<IActionResult> Index(string? filter)
    {
      bool find = false;
      if (filter == null)
        return _context.RSSFeeds != null ?
                    View(await _context.RSSFeeds.ToListAsync()) :
                    Problem("Entity set 'RSSFeedContext.RSSFeeds'  is null.");
      else
      {
        List<RssFeeds> feeds = await _context.RSSFeeds.ToListAsync();
        for (int i =feeds.Count - 1; i >= 0; i--)
        {
          find = false;

          if (feeds[i].Name.Contains(filter))
            continue;

          List<Article> articles = _xmlHelper.GetXmlFromUrl(feeds[i].Url);

          foreach (Article article in articles)
          {
            if (!find)
              if (article.Title.Contains(filter))
                find = true;
          }
          if (!find)
          {
            feeds.Remove(feeds[i]);
          }
        }
        return View(feeds);
      }
    }

    // GET: RssFeeds/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null || _context.RSSFeeds == null)
      {
        return NotFound();
      }

      var rssFeeds = await _context.RSSFeeds
          .FirstOrDefaultAsync(m => m.Id == id);
      if (rssFeeds == null)
      {
        return NotFound();
      }

      List<Article> articles = _xmlHelper.GetXmlFromUrl(rssFeeds.Url);

      if (articles.Count > 0)
        return View(articles);
      else
        return RedirectToAction("Nastala chyba při zpracování URL");
    }

    public async Task<IActionResult> ArticleList(int? id, DateTime? fromDate, DateTime? toDate)
    {
      if (id == null || _context.RSSFeeds == null)
      {
        return NotFound();
      }

      var rssFeeds = await _context.RSSFeeds
          .FirstOrDefaultAsync(m => m.Id == id);
      if (rssFeeds == null)
      {
        return NotFound();
      }
      ViewBag.Id = id;
      List<Article> articles = _xmlHelper.GetXmlFromUrl(rssFeeds.Url);

      if (articles.Count > 0)
      {
        if (fromDate > DateTime.MinValue && toDate > DateTime.MinValue)
        {
          articles = articles.Where(x => x.PublishDate >= fromDate && x.PublishDate <= toDate).ToList();
          return View(articles);
        }
        else if (fromDate > DateTime.MinValue)
        {
          articles = articles.Where(x => x.PublishDate >= fromDate).ToList();
          return View(articles);
        }
        else if (toDate > DateTime.MinValue)
        {
          articles = articles.Where(x => x.PublishDate <= toDate).ToList();
          return View(articles);
        }
        else
          return View(articles);
      }
      else
        return RedirectToAction("Nastala chyba při zpracování URL");
    }

    // GET: RssFeeds/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: RssFeeds/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Url")] RssFeeds rssFeeds)
    {
      if (ModelState.IsValid)
      {
        _context.Add(rssFeeds);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(rssFeeds);
    }

    // GET: RssFeeds/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null || _context.RSSFeeds == null)
      {
        return NotFound();
      }

      var rssFeeds = await _context.RSSFeeds.FindAsync(id);
      if (rssFeeds == null)
      {
        return NotFound();
      }
      return View(rssFeeds);
    }

    // POST: RssFeeds/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url")] RssFeeds rssFeeds)
    {
      if (id != rssFeeds.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(rssFeeds);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!RssFeedsExists(rssFeeds.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(rssFeeds);
    }

    // GET: RssFeeds/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null || _context.RSSFeeds == null)
      {
        return NotFound();
      }

      var rssFeeds = await _context.RSSFeeds
          .FirstOrDefaultAsync(m => m.Id == id);
      if (rssFeeds == null)
      {
        return NotFound();
      }

      return View(rssFeeds);
    }

    // POST: RssFeeds/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      if (_context.RSSFeeds == null)
      {
        return Problem("Entity set 'RSSFeedContext.RSSFeeds'  is null.");
      }
      var rssFeeds = await _context.RSSFeeds.FindAsync(id);
      if (rssFeeds != null)
      {
        _context.RSSFeeds.Remove(rssFeeds);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteSelected(int[] selectedIds)
    {
      if (selectedIds != null && selectedIds.Length > 0)
      {
        var selectedData = _context.RSSFeeds.Where(d => selectedIds.Contains(d.Id));

        _context.RSSFeeds.RemoveRange(selectedData);
        await _context.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Index));
    }

    private bool RssFeedsExists(int id)
    {
      return (_context.RSSFeeds?.Any(e => e.Id == id)).GetValueOrDefault();
    }
  }
}
