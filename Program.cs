using System.Globalization;
using Microsoft.AspNetCore.Localization;
using MyMarkdownBlog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddLocalization();
var defaultCulture = new CultureInfo("fa-IR");
var supportedCultures = new[] { defaultCulture };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddScoped<IBlogService, BlogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Set the default culture to Persian (fa-IR)
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");
app.UseRequestLocalization(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
