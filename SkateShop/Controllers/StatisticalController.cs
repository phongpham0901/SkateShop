using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SkateShop.Models;
using SkateShop.Services;
using System.Data;

namespace SkateShop.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class StatisticalController : Controller
    {
        private readonly ApplicationDbContext context;

        public StatisticalController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var statistics = await context.Items
                .Include(o => o.Product)
                .Include(o => o.Order) // Load Order để kiểm tra OrderStatus
                .Where(o => o.Order.OrderStatus == "Delivered") // Lọc chỉ lấy đơn hàng đã giao
                .GroupBy(o => o.Product.Id)
                .Select(g => new OrderItem
                {
                    Product = g.First().Product,
                    Quantity = g.Sum(o => o.Quantity),
                    UnitPrice = g.Sum(o => o.Quantity * o.UnitPrice)
                })
                .OrderByDescending(s => s.UnitPrice) // Đúng cú pháp
                .ToListAsync();

            var totalRevenueOfAllProducts = statistics.Sum(s => s.UnitPrice);
            ViewBag.TotalRevenueOfAllProducts = totalRevenueOfAllProducts;

            return View(statistics);
        }

        public async Task<IActionResult> Monthly()
        {
            var currentYear = DateTime.Now.Year;

            var monthlyStatistics = await context.Items
                .Include(i => i.Order)
                .Where(i => i.Order.OrderStatus == "Delivered" && i.Order.CreatedAt.Year == currentYear)
                .GroupBy(i => i.Order.CreatedAt.Month)
                .Select(g => new MonthlyRevenueViewModel
                {
                    Month = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderBy(g => g.Month)
                .ToListAsync();

            return View(monthlyStatistics);
        }



        public async Task<IActionResult> ExportToExcel()
        {
            var statistics = await context.Items
                .Include(o => o.Product)
                .Include(o => o.Order)
                .Where(o => o.Order.OrderStatus == "Delivered")
                .GroupBy(o => o.Product.Id)
                .Select(g => new
                {
                    ProductName = g.First().Product.Name,
                    Brand = g.First().Product.Brand,
                    Type = g.First().Product.Type,
                    TotalQuantity = g.Sum(o => o.Quantity),
                    TotalRevenue = g.Sum(o => o.Quantity * o.UnitPrice)
                })
                .OrderByDescending(s => s.TotalRevenue) // Đúng cú pháp
                .ToListAsync();

            var totalRevenueOfAllProducts = statistics.Sum(s => s.TotalRevenue);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Revenue Statistics");

                // Header
                worksheet.Cells[1, 1].Value = "Product";
                worksheet.Cells[1, 2].Value = "Brand";
                worksheet.Cells[1, 3].Value = "Brand";
                worksheet.Cells[1, 4].Value = "Quantity";
                worksheet.Cells[1, 5].Value = "Total Revenue";
                worksheet.Cells[1, 6].Value = "Total Revenue Of All Products";

                // Dữ liệu
                int row = 2;
                foreach (var item in statistics)
                {
                    worksheet.Cells[row, 1].Value = item.ProductName;
                    worksheet.Cells[row, 2].Value = item.Brand;
                    worksheet.Cells[row, 3].Value = item.Type;
                    worksheet.Cells[row, 4].Value = item.TotalQuantity;
                    worksheet.Cells[row, 5].Value = item.TotalRevenue;
                    row++;
                }

                worksheet.Cells[row, 6].Value = totalRevenueOfAllProducts;

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ThongKeDoanhThu.xlsx");
            }
        }


        public async Task<IActionResult> ExportMonthlyToExcel()
        {
            var currentYear = DateTime.Now.Year;

            var monthlyStatistics = await context.Items
                .Include(i => i.Order)
                .Where(i => i.Order.OrderStatus == "Delivered" && i.Order.CreatedAt.Year == currentYear)
                .GroupBy(i => i.Order.CreatedAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderBy(g => g.Month)
                .ToListAsync();

            var totalRevenueAllMonths = monthlyStatistics.Sum(m => m.TotalRevenue);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Monthly Revenue");

                // Header
                worksheet.Cells[1, 1].Value = "Tháng";
                worksheet.Cells[1, 2].Value = "Tổng số lượng";
                worksheet.Cells[1, 3].Value = "Tổng doanh thu";
                worksheet.Cells[1, 4].Value = "Tổng doanh thu cả năm";

                int row = 2;
                foreach (var item in monthlyStatistics)
                {
                    worksheet.Cells[row, 1].Value = $"Tháng {item.Month}";
                    worksheet.Cells[row, 2].Value = item.TotalQuantity;
                    worksheet.Cells[row, 3].Value = item.TotalRevenue;
                    row++;
                }

                worksheet.Cells[2, 4].Value = totalRevenueAllMonths;

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ThongKeThang_{currentYear}.xlsx");
            }
        }



    }
}
