using Microsoft.AspNetCore.Mvc;

namespace SmartAdopt.Controllers
{
    public class SitemapController : Controller
    {
        public IActionResult Index()
        {
            var sitemap = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
            <url>
                <loc>https://localhost:7249/</loc>
                <lastmod>2025-06-04</lastmod>
                <changefreq>daily</changefreq>
                <priority>1.0</priority>
            </url>
            <url>
                <loc>https://localhost:7249/Clients/Chestionar</loc>
                <lastmod>2025-06-04</lastmod>
                <changefreq>weekly</changefreq>
                <priority>0.8</priority>
            </url>
            <url>
                <loc>https://localhost:7249/Postares/Index</loc>
                <lastmod>2025-06-04</lastmod>
                <changefreq>weekly</changefreq>
                <priority>0.8</priority>
            </url>
            <url>
                <loc>https://localhost:7249/Admins/Raport</loc>
                <lastmod>2025-06-04</lastmod>
                <changefreq>daily</changefreq>
                <priority>0.5</priority>
            </url>
        </urlset>";
            return Content(sitemap, "text/xml");
        }
    }
}
