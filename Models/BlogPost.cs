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

    // Series-related metadata (optional)
    // The display name of the series this post belongs to (e.g. "Getting Started Series")
    public string Series { get; set; } = string.Empty;

    // Slug for the series (derived from Series) for linking
    public string SeriesSlug { get; set; } = string.Empty;

    // Optional index/position within the series (smaller means earlier)
    public int? SeriesIndex { get; set; }
}
