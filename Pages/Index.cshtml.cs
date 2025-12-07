using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMarkdownBlog.Services;
using MyMarkdownBlog.Models;
using System.Collections.Generic;

namespace MyMarkdownBlog.Pages;

public class IndexModel : PageModel
{
    private readonly IBlogService _blogService;

    public IndexModel(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public List<BlogPost> RecentPosts { get; set; } = new();
    public string AboutMeContent { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        // Get recent blog posts (limit to 3 most recent)
        var allPosts = await _blogService.GetAllPostsAsync();
        RecentPosts = allPosts.Take(3).ToList();

        // Set about me content
        AboutMeContent = "Welcome to my personal blog! I'm a passionate developer who loves sharing knowledge and experiences about technology, programming, and software development. Here you'll find articles, tutorials, and insights on various topics in the tech world.";
    }
}
