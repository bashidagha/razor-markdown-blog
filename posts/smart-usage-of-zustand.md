---
title: "استفاده از zustand برای بهبود مدیریت state ها"
date: "2025-12-05"
author: "فرشید"
image: "/images/blog/zustand.webp"
---

## چرا zustand ؟

وقتی صحبت از مدیریت stateها در اپلیکیشن‌های react ای سناریوهای مختلف و کاربری‌های مختلفی ایجاد میشه که توی مقاله فلان راجه بهش مفصل صحبت کردیم.
همونطور که اونجا گفتیم، برای مدیریت s

کار با zustand خیلی راحت و باحاله، الو از همه بگذارید راجع به فلسفه کارکردی zustand یکم صحبت کنیم و اینکه چجوری داره کار می‌کنه؟

## فلسفه عملکردی zustand

وقتی شما با zustand یک store در جایی می سازی در حقیقت در run-time جاوااسکریپت یه محدوده ای می‌سازی که داخلش چیزهایی که می‌خوای رو ذخیره می‌کنی. یعنی یه چیزی شبیه این شکل زیری

![Zustand run-time assets](/images/assets/zustand1.webp)

اول بگم ‌lifecycle این store کاملا به ‌‌‌run-time شما بستگی داره یعنی اگر صفحه رو reload کنید قاعدتا این store پاک میشه مگر اینکه جای دیگه ای هم persist بشه که موضوع بحث ما فعلا نیست.
همونطور که توی شکل بالام مشخصه اپلیکیشن شما هر حرکتی بزنه دخلی(!) به این store به طور معمول نداره جون space یا فضای حافظه و run-time شون جداست.

حالا از این ببعدش جالب میشه، شما وقتی zustand رو به طور مثال در برنامه react تون استفاده می‌کنید یه wrapper ای از جنس stateهای react دورش ایجاد میشه که فرآیندهای آپدیت و rendering در react بدین وسیله handle میشه.

شکل زیر تقریبی از چیزی است که اتفاق می‌افته. به عبارتی برای اینکه react بتونه rendering رو برعهده بگیره نیازی به چیزی که براش آشناست یعنی همون stateها داره. حالا ما هربار که useStore رو استفاده کنیم(بعدا خواهیم گفت)، در حقیقت کنترل re-render و render رو برعهده می‌گیریم.

![Zustand React model](/images/assets/zustand2.webp)

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
