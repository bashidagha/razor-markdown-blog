using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMarkdownBlog.Models;
using MyMarkdownBlog.Services;

namespace MyMarkdownBlog.Pages;

public class PostModel : PageModel
{
    private readonly IBlogService _blogService;

    public PostModel(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public BlogPost? Post { get; set; }

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return RedirectToPage("/Blog");
        }

        Post = await _blogService.GetPostBySlugAsync(slug);

        if (Post == null)
        {
            return NotFound();
        }

        return Page();
    }
}
