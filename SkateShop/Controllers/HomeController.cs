using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkateShop.Models;
using SkateShop.Services;
using System.Diagnostics;
using System.Drawing.Printing;

namespace SkateShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 2;
        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).Take(4).ToList();

            var topProducts = context.Items
    .Include(oi => oi.Product)
    .Include(oi => oi.Order)
    .Where(oi => oi.Order.OrderStatus == "Delivered")
    .AsEnumerable() 
    .GroupBy(oi => oi.Product.Id)
    .Select(g => new
    {
        Product = g.First().Product,
        TotalQuantity = g.Sum(oi => oi.Quantity)
    })
    .OrderByDescending(x => x.TotalQuantity)
    .Take(4)
    .Select(x => x.Product)
    .ToList();


            ViewData["TopProducts"] = topProducts;

            return View(products);
        }

        public IActionResult News(int pageIndex, string? search)
        {
            IQueryable<Articlecs> query = context.Articlecs;

            if (search != null)
            {
                query = query.Where(p => p.Title.Contains(search));
            }


            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var a = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            return View(a);
        }

        public IActionResult Policy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
