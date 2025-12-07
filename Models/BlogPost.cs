using System.ComponentModel.DataAnnotations;

namespace MyMarkdownBlog.Models;

public class BlogPost
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public string Excerpt { get; set; } = string.Empty;

    public string Author { get; set; } = "Admin";

    public string Image { get; set; } = string.Empty;
}
