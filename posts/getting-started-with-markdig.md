---
title: "Getting Started with Markdig: A Powerful Markdown Parser"
date: "2025-12-05"
author: "Admin"
---

# Getting Started with Markdig

Markdig is a fast, powerful, and lightweight Markdown processor for .NET. It's designed to be fast, extensible, and standards-compliant.

## Why Choose Markdig?

Markdig offers several advantages:

-   **Performance**: Extremely fast parsing and rendering
-   **Standards Compliance**: Follows CommonMark specifications
-   **Extensibility**: Easy to add custom extensions
-   **Safety**: Built-in XSS protection

## Installation

Installing Markdig is straightforward with NuGet:

```bash
Install-Package Markdig
```

Or using the .NET CLI:

```bash
dotnet add package Markdig
```

## Basic Usage

Here's how to use Markdig in your C# code:

```csharp
using Markdig;

// Simple markdown to HTML conversion
string markdown = "# Hello World\n\nThis is a **bold** statement!";
string html = Markdown.ToHtml(markdown);

// With advanced extensions
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

string advancedHtml = Markdown.ToHtml(markdown, pipeline);
```

## Available Extensions

Markdig comes with many built-in extensions:

### Advanced Extensions

-   **Auto-identifier**: Automatically adds IDs to headings
-   **Custom Containers**: Create custom block containers
-   **Definition Lists**: Support for definition lists
-   **Emphasis Extra**: Extended emphasis syntax
-   **Figures**: Figure and figcaption support
-   **Footers**: Document footers
-   **Footnotes**: Footnote support
-   **Grid Tables**: Grid table syntax
-   **Mathematics**: LaTeX-style math expressions
-   **Media**: Automatic media embedding
-   **Pipe Tables**: Pipe-separated tables
-   **ListExtras**: Enhanced list features
-   **Yaml**: YAML frontmatter support

### Example with Tables

```markdown
| Feature    | Status |
| ---------- | ------ |
| Fast       | ✅     |
| Extensible | ✅     |
| Safe       | ✅     |
```

### Example with Code Fencing

```javascript
function greet(name) {
    console.log(`Hello, ${name}!`);
}

greet("World");
```

## Custom Extensions

You can create custom extensions for Markdig:

```csharp
public class MyCustomExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Add your custom processing
    }

    public void Setup(MarkdownParser parser)
    {
        // Add custom parsers
    }
}
```

---

Markdig is an excellent choice for any .NET project that needs to process Markdown content. Its combination of speed, features, and extensibility makes it one of the best Markdown parsers available for .NET.
