using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMarkdownBlog.Models;
using MyMarkdownBlog.Services;

namespace MyMarkdownBlog.Pages;

public class BlogModel : PageModel
{
    private readonly IBlogService _blogService;

    public BlogModel(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public List<BlogPost> Posts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Posts = await _blogService.GetAllPostsAsync();
    }
}
