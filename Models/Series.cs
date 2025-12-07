using System.ComponentModel.DataAnnotations;
namespace MyMarkdownBlog.Models;

public class Series
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public List<BlogPost> Posts { get; set; } = new();
}
