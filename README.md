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
-   **Beautiful design** - Clean, responsive layout with TailwindCSS
-   **Zero setup** - Just write Markdown files and they appear as blog posts!

## 🚀 Quick Start

### 1. Clone the repository

```bash
git clone https://github.com/bashidagha/MyMarkdownBlog.git
cd MyMarkdownBlog
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Run the blog

```bash
dotnet run
```

Open your browser and visit: **https://localhost:5001**

That's it! 🎉 Your blog is now running.

## 📝 Adding New Posts

Adding a new blog post is super easy! Just create a `.md` file in the `posts/` folder.

### Example Post

Create a file called `my-first-post.md` in the `posts/` folder:

````markdown
---
title: "My First Blog Post"
date: "2025-12-07"
author: "Your Name"
---

# Welcome to My Blog!

This is my first post. Here's some content:

## Features I Love

-   Writing in Markdown is easy
-   Code highlighting works great
-   Frontmatter makes organization simple

### Code Example

```csharp
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, Blog World!");
    }
}
```
````

Check out the [Markdig documentation](https://github.com/xoofx/markdig) for more Markdown features!

````

**That's it!** Save the file and refresh your browser - your new post will appear automatically! ✨

## 🔍 SEO Features

This blog is built with SEO in mind:

-   **Automatic Meta Tags** - Each post gets proper title, description, and Open Graph tags
-   **Clean URLs** - Posts use SEO-friendly slugs based on titles
-   **Structured Data** - Posts include author and publish date metadata
-   **Fast Loading** - Build-time HTML generation ensures lightning-fast page loads
-   **Mobile-Friendly** - Responsive design works perfectly on all devices

### SEO Best Practices

When writing posts, consider these tips:

1. **Write compelling titles** - Your frontmatter title becomes the page title and meta description
2. **Use descriptive slugs** - The system automatically generates clean URLs from your titles
3. **Add author information** - Include your name in the frontmatter for better authorship
4. **Write meaningful excerpts** - The first 200 characters become your meta description

## 💅 TailwindCSS Benefits

This blog uses TailwindCSS for modern, beautiful styling:

-   **Utility-First Approach** - Build custom designs without leaving your HTML
-   **Responsive Design** - Mobile-first responsive classes that work out of the box
-   **Fast Development** - Rapidly build custom user interfaces
-   **No CSS Bloat** - Only includes the CSS you actually use
-   **Consistent Design** - Built-in design system with consistent spacing and typography
-   **Dark Mode Ready** - Easy to add dark mode support with Tailwind's utilities

### Customizing Styles

To modify the blog's appearance:

1. Edit `wwwroot/css/tailwind.css` to add your custom styles
2. Run `npm run build:css` to compile the CSS
3. Or use `npm run watch:css` to automatically rebuild when you make changes

## 🛠️ Development

### Running locally
```bash
dotnet run
````

### Building CSS (if you modify Tailwind)

```bash
npm install
npm run build:css
```

### Watching CSS changes during development

```bash
npm run watch:css
```

## 📁 Project Structure

```
MyMarkdownBlog/
├── posts/              # Your Markdown blog posts go here
├── wwwroot/
│   └── compiled-posts/ # Auto-generated HTML files
├── Services/           # Blog processing logic
├── Models/             # Data models
└── Pages/              # Razor Pages (blog index, individual posts)
```

## 🤝 Contributing

Contributions are welcome! Here's how:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 🙏 Acknowledgments

-   Built with [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
-   Markdown processing by [Markdig](https://github.com/xoofx/markdig)
-   Styling with [TailwindCSS](https://tailwindcss.com/)

---

**Happy blogging!** 🚀
