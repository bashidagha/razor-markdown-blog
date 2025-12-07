# ASP.NET Core Markdown Blog - Fast, SEO-Friendly Static Site Generator 📝

A simple, fast, and beautiful Markdown-powered blog built with ASP.NET Core and Markdig.

![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![C#](https://img.shields.io/badge/C%23-10.0-blue)
![Markdig](https://img.shields.io/badge/Markdig-0.44.0-green)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-3.4.0-blue)

## ✨ Features

-   **Markdown-powered** - Write posts in simple Markdown
-   **Build-time conversion** - Fast performance with pre-compiled HTML
-   **Frontmatter support** - Add metadata to your posts (title, date, author)
-   **SEO-friendly** - Automatic meta tags, Open Graph, and Twitter Cards
-   **TailwindCSS styling** - Modern, utility-first CSS framework for beautiful, responsive designs

# MyMarkdownBlog — Simple Markdown blog (ASP.NET Core)

A compact, fast blog that turns Markdown files into pre-rendered HTML. Write posts in `posts/` and the site compiles them with Markdig. Now supports "Series": group related posts using `series` and `series_index` in frontmatter.

Highlights

-   Fast, pre-compiled HTML for great performance and SEO
-   Markdown with frontmatter (title, date, author, image, series, series_index)
-   Table of Contents auto-generated and inserted into posts
-   Series support: list series at `/Series` and view ordered posts per series
-   Uses TailwindCSS for a clean, responsive layout

Quick start

1. Clone:

```bash
git clone https://github.com/bashidagha/MyMarkdownBlog.git
cd MyMarkdownBlog
```

2. Restore & run:

```bash
dotnet restore
dotnet run
```

Open `http://localhost:5077` (or the URL shown in the console).

Add posts

-   Create a file in `posts/` with YAML frontmatter, for example:

```md
---
title: "My Post"
date: "2025-12-07"
author: "You"
series: "Getting Started"
series_index: 1
---

# My Post

Content...
```

Series behavior

-   Posts that share the same `series` value are grouped into a series page.
-   Use `series_index` (integer) to control ordering; when missing, posts are sorted by date.
-   Example series posts have been added in `posts/` as `example-series-part-1-introduction.md` and `example-series-part-2-next-steps.md`.

Development notes

-   Compiled HTML files are written to `wwwroot/compiled-posts/` during startup.
-   To regenerate compiled posts, start the app (`dotnet run`) or call the blog service refresh endpoint if you expose one.

Contributing

-   Fork, branch, and open a PR. Keep changes focused and add tests where appropriate.

License

-   MIT

Enjoy — write more, ship faster 🚀
