using Markdig;
using MyMarkdownBlog.Models;
using System.Text.RegularExpressions;
using System.Net;

namespace MyMarkdownBlog.Services;

public interface IBlogService
{
    Task<List<BlogPost>> GetAllPostsAsync();
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task RefreshPostsAsync();
    Task<List<Series>> GetAllSeriesAsync();
    Task<Series?> GetSeriesBySlugAsync(string slug);
}

public class BlogService : IBlogService
{
    private readonly string _postsDirectory;
    private readonly string _compiledPostsPath;
    private readonly object _lock = new object();
    private List<BlogPost> _posts = new();
    private List<Series> _series = new();
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
            // Build series index
            var seriesLookup = new Dictionary<string, Series>(StringComparer.OrdinalIgnoreCase);

            foreach (var p in posts)
            {
                if (string.IsNullOrWhiteSpace(p.Series)) continue;
                var slug = !string.IsNullOrWhiteSpace(p.SeriesSlug) ? p.SeriesSlug : GenerateSlug(p.Series);

                if (!seriesLookup.TryGetValue(slug, out var s))
                {
                    s = new Series { Name = p.Series, Slug = slug };
                    seriesLookup[slug] = s;
                }

                s.Posts.Add(p);
            }

            // Sort posts in each series by SeriesIndex then Date
            foreach (var s in seriesLookup.Values)
            {
                s.Posts = s.Posts.OrderBy(p => p.SeriesIndex ?? int.MaxValue).ThenBy(p => p.Date).ToList();
            }

            _series = seriesLookup.Values.OrderBy(s => s.Name).ToList();
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
        var image = frontmatter.GetValueOrDefault("image") ?? string.Empty;

        if (!DateTime.TryParse(dateStr, out var date))
        {
            date = File.GetCreationTime(filePath);
        }

        var slug = GenerateSlug(title);
        var htmlContent = ConvertMarkdownToHtml(markdownContent);

        // Parse optional series frontmatter
        var seriesName = frontmatter.GetValueOrDefault("series") ?? string.Empty;
        var seriesIndexStr = frontmatter.GetValueOrDefault("series_index") ?? string.Empty;
        int? seriesIndex = null;
        if (int.TryParse(seriesIndexStr, out var idx)) seriesIndex = idx;

        // Generate and insert TOC (after first H1 if present)
        var tocHtml = GenerateTocHtml(htmlContent);
        if (!string.IsNullOrWhiteSpace(tocHtml))
        {
            var h1Regex = new Regex("<h1\\b.*?>.*?</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (h1Regex.IsMatch(htmlContent))
            {
                htmlContent = h1Regex.Replace(htmlContent, m => m.Value + "\n" + tocHtml, 1);
            }
            else
            {
                htmlContent = tocHtml + "\n" + htmlContent;
            }
        }

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
            Author = author,
            Image = image
            ,
            Series = seriesName,
            SeriesSlug = string.IsNullOrWhiteSpace(seriesName) ? string.Empty : GenerateSlug(seriesName),
            SeriesIndex = seriesIndex
        };
    }

    public async Task<List<Series>> GetAllSeriesAsync()
    {
        if (!_isLoaded)
            await LoadPostsAsync();

        return _series.OrderBy(s => s.Name).ToList();
    }

    public async Task<Series?> GetSeriesBySlugAsync(string slug)
    {
        if (!_isLoaded)
            await LoadPostsAsync();

        return _series.FirstOrDefault(s => s.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
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

    private string GenerateTocHtml(string html)
    {
        // Find headings with id attributes (h1..h6)
        var headingRegex = new Regex("<h([1-6])\\s+id=\"([^\"]+)\">(.+?)</h\\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var matches = headingRegex.Matches(html);

        var headings = new List<(int level, string id, string text)>();

        foreach (Match m in matches)
        {
            if (!m.Success) continue;
            if (!int.TryParse(m.Groups[1].Value, out var level)) continue;
            var id = m.Groups[2].Value;
            var innerHtml = m.Groups[3].Value;
            // Strip any inner tags and decode HTML entities
            var text = Regex.Replace(innerHtml, "<.*?>", "");
            text = WebUtility.HtmlDecode(text).Trim();

            // Skip the main H1 (we'll show TOC for H2-H4 by default)
            if (level >= 2 && level <= 4)
            {
                headings.Add((level, id, text));
            }
        }

        if (headings.Count == 0)
            return string.Empty;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<nav class=\"toc-container bg-gray-50 border rounded-md p-4 mb-6\" aria-label=\"Table of contents\">");
        sb.AppendLine("  <p class=\"font-semibold mb-2\">Contents</p>");

        // Build nested list based on heading levels
        int baseLevel = headings.Min(h => h.level);
        int currentLevel = baseLevel;

        sb.AppendLine("  <ul class=\"toc-list\">");

        foreach (var h in headings)
        {
            while (h.level > currentLevel)
            {
                sb.AppendLine("    <ul>");
                currentLevel++;
            }

            while (h.level < currentLevel)
            {
                sb.AppendLine("    </ul>");
                currentLevel--;
            }

            sb.AppendLine($"    <li class=\"toc-item toc-h{h.level}\"><a class=\"toc-link text-sm text-blue-600 hover:underline\" href=\"#{h.id}\">{WebUtility.HtmlEncode(h.text)}</a></li>");
        }

        while (currentLevel > baseLevel)
        {
            sb.AppendLine("    </ul>");
            currentLevel--;
        }

        sb.AppendLine("  </ul>");
        sb.AppendLine("</nav>");

        return sb.ToString();
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
