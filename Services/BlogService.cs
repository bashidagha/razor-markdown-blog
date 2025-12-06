using Markdig;
using MyMarkdownBlog.Models;
using System.Text.RegularExpressions;

namespace MyMarkdownBlog.Services;

public interface IBlogService
{
    Task<List<BlogPost>> GetAllPostsAsync();
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task RefreshPostsAsync();
}

public class BlogService : IBlogService
{
    private readonly string _postsDirectory;
    private readonly string _compiledPostsPath;
    private readonly object _lock = new object();
    private List<BlogPost> _posts = new();
    private bool _isLoaded = false;

    public BlogService(IWebHostEnvironment environment)
    {
        _postsDirectory = Path.Combine(environment.ContentRootPath, "posts");
        _compiledPostsPath = Path.Combine(environment.ContentRootPath, "wwwroot", "compiled-posts");

        // Ensure directories exist
        if (!Directory.Exists(_postsDirectory))
            Directory.CreateDirectory(_postsDirectory);

        if (!Directory.Exists(_compiledPostsPath))
            Directory.CreateDirectory(_compiledPostsPath);
    }

    public async Task<List<BlogPost>> GetAllPostsAsync()
    {
        if (!_isLoaded)
        {
            await LoadPostsAsync();
        }

        return _posts.OrderByDescending(p => p.Date).ToList();
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        if (!_isLoaded)
        {
            await LoadPostsAsync();
        }

        return _posts.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task RefreshPostsAsync()
    {
        lock (_lock)
        {
            _isLoaded = false;
            _posts.Clear();
        }

        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        lock (_lock)
        {
            if (_isLoaded) return;
        }

        var markdownFiles = Directory.GetFiles(_postsDirectory, "*.md", SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTime)
            .ToList();

        var posts = new List<BlogPost>();

        foreach (var file in markdownFiles)
        {
            try
            {
                var post = await ProcessMarkdownFileAsync(file);
                if (post != null)
                {
                    posts.Add(post);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue processing other files
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        }

        lock (_lock)
        {
            _posts = posts;
            _isLoaded = true;
        }
    }

    private async Task<BlogPost?> ProcessMarkdownFileAsync(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        var (frontmatter, markdownContent) = ParseFrontmatter(content);

        if (frontmatter == null)
        {
            // Try to extract title from first heading if no frontmatter
            var firstLine = markdownContent.Split('\n').FirstOrDefault(l => l.Trim().StartsWith('#'));
            if (firstLine != null)
            {
                frontmatter = new Dictionary<string, string>
                {
                    ["title"] = firstLine.TrimStart('#').Trim(),
                    ["date"] = File.GetCreationTime(filePath).ToString("yyyy-MM-dd")
                };
            }
            else
            {
                return null;
            }
        }

        var title = frontmatter.GetValueOrDefault("title") ?? Path.GetFileNameWithoutExtension(filePath);
        var dateStr = frontmatter.GetValueOrDefault("date") ?? File.GetCreationTime(filePath).ToString("yyyy-MM-dd");
        var author = frontmatter.GetValueOrDefault("author") ?? "Admin";

        if (!DateTime.TryParse(dateStr, out var date))
        {
            date = File.GetCreationTime(filePath);
        }

        var slug = GenerateSlug(title);
        var htmlContent = ConvertMarkdownToHtml(markdownContent);
        var excerpt = GenerateExcerpt(markdownContent);

        // Save compiled HTML
        var compiledPath = Path.Combine(_compiledPostsPath, $"{slug}.html");
        await File.WriteAllTextAsync(compiledPath, htmlContent);

        return new BlogPost
        {
            Title = title,
            Slug = slug,
            Date = date,
            Content = htmlContent,
            Excerpt = excerpt,
            Author = author
        };
    }

    private (Dictionary<string, string>?, string) ParseFrontmatter(string content)
    {
        var lines = content.Split('\n');
        if (lines.Length < 3 || !lines[0].Trim().Equals("---", StringComparison.Ordinal))
        {
            return (null, content);
        }

        var frontmatterEndIndex = -1;
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Trim().Equals("---", StringComparison.Ordinal))
            {
                frontmatterEndIndex = i;
                break;
            }
        }

        if (frontmatterEndIndex == -1)
        {
            return (null, content);
        }

        var frontmatterLines = lines.Skip(1).Take(frontmatterEndIndex - 1).ToList();
        var frontmatter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in frontmatterLines)
        {
            var colonIndex = line.IndexOf(':');
            if (colonIndex > 0)
            {
                var key = line.Substring(0, colonIndex).Trim();
                var value = line.Substring(colonIndex + 1).Trim().Trim('"', '\'');
                frontmatter[key] = value;
            }
        }

        var markdownContent = string.Join('\n', lines.Skip(frontmatterEndIndex + 1));
        return (frontmatter, markdownContent);
    }

    private string ConvertMarkdownToHtml(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        return Markdown.ToHtml(markdown, pipeline);
    }

    private string GenerateExcerpt(string markdown, int maxLength = 200)
    {
        var plainText = Regex.Replace(markdown, @"#.*?\n|\*\*.*?\*\*|`.*?`|\[.*?\]\(.*?\)|!\[.*?\]\(.*?\)", "");
        plainText = Regex.Replace(plainText, @"<.*?>", "");
        plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

        if (plainText.Length <= maxLength)
            return plainText;

        var excerpt = plainText.Substring(0, maxLength);
        var lastSpace = excerpt.LastIndexOf(' ');

        if (lastSpace > 0)
            excerpt = excerpt.Substring(0, lastSpace);

        return excerpt + "...";
    }

    private string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }
}
