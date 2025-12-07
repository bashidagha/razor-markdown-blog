using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMarkdownBlog.Models;
using MyMarkdownBlog.Services;

namespace MyMarkdownBlog.Pages;

public class SeriesModel : PageModel
{
    private readonly IBlogService _blogService;

    public SeriesModel(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public List<Series> SeriesList { get; set; } = new();
    public Series? CurrentSeries { get; set; }

    public async Task<IActionResult> OnGetAsync(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            SeriesList = await _blogService.GetAllSeriesAsync();
            return Page();
        }

        CurrentSeries = await _blogService.GetSeriesBySlugAsync(slug!);
        if (CurrentSeries == null)
            return NotFound();

        return Page();
    }
}
