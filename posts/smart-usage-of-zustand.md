---
title: "استفاده از zustand برای بهبود مدیریت state ها"
date: "2025-12-05"
author: "فرشید"
image: "/images/blog/zustand.webp"
---

# شروع به کار با Markdig

Markdig یک پردازشگر مارک‌داون سریع، قدرتمند و سبک برای .NET است. این کتابخانه برای سرعت، قابلیت توسعه و تطابق با استانداردها طراحی شده است.

## چرا Markdig؟

Markdig چندین مزیت ارائه می‌دهد:

-   **عملکرد**: پردازش و رندر بسیار سریع
-   **تطابق با استانداردها**: پیروی از مشخصات CommonMark
-   **قابلیت توسعه**: افزودن افزونه‌های سفارشی ساده است
-   **امنیت**: شامل محافظت‌های پایه‌ای در برابر XSS

## نصب

نصب Markdig با NuGet ساده است:

```bash
Install-Package Markdig
```

یا با استفاده از CLI دات‌نت:

```bash
dotnet add package Markdig
```

## استفاده ساده

در اینجا نحوه استفاده از Markdig در کد C# آورده شده است:

```csharp
using Markdig;

// تبدیل ساده مارک‌داون به HTML
string markdown = "# Hello World\n\nThis is a **bold** statement!";
string html = Markdown.ToHtml(markdown);

// با افزونه‌های پیشرفته
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

string advancedHtml = Markdown.ToHtml(markdown, pipeline);
```

## افزونه‌های موجود

Markdig دارای بسیاری از افزونه‌های داخلی است:

### افزونه‌های پیشرفته

-   **Auto-identifier**: به‌طور خودکار شناسه به سرفصل‌ها اضافه می‌کند
-   **Custom Containers**: ایجاد کانتینرهای بلوکی سفارشی
-   **Definition Lists**: پشتیبانی از فهرست‌های تعریفی
-   **Emphasis Extra**: نگارش توسعه‌یافته‌ی تأکید
-   **Figures**: پشتیبانی از figure و figcaption
-   **Footers**: پاورقی‌های سند
-   **Footnotes**: پشتیبانی از پانوشت
-   **Grid Tables**: نگارش جدول شبکه‌ای
-   **Mathematics**: عبارات ریاضی به سبک LaTeX
-   **Media**: جاسازی خودکار رسانه
-   **Pipe Tables**: جداول جداشده با خط لوله
-   **ListExtras**: قابلیت‌های اضافی فهرست
-   **Yaml**: پشتیبانی از فرانت‌متای YAML

### مثال با جداول

```markdown
| Feature    | Status |
| ---------- | ------ |
| Fast       | ✅     |
| Extensible | ✅     |
| Safe       | ✅     |
```

### مثال با بلوک کد

```javascript
function greet(name) {
    console.log(`Hello, ${name}!`);
}

greet("World");
```

## افزونه‌های سفارشی

می‌توانید افزونه‌های سفارشی برای Markdig ایجاد کنید:

```csharp
public class MyCustomExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // افزودن پردازش سفارشی
    }

    public void Setup(MarkdownParser parser)
    {
        // افزودن پارسرهای سفارشی
    }
}
```

---

Markdig انتخاب بسیار خوبی برای هر پروژه .NET است که نیاز به پردازش محتوای مارک‌داون دارد. ترکیب سرعت، امکانات و قابلیت توسعه آن را به یکی از بهترین پردازشگرهای مارک‌داون برای .NET تبدیل می‌کند.
