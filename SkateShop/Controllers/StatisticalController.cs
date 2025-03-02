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
                .Include(o => o.Product) // Load sản phẩm
                .GroupBy(o => o.Product.Id)
                .Select(g => new OrderItem
                {
                    Product = g.First().Product, // Lấy thông tin sản phẩm
                    Quantity = g.Sum(o => o.Quantity), // Tổng số lượng bán
                    UnitPrice = g.Sum(o => o.Quantity * o.UnitPrice) // Tổng doanh thu
                })
                .OrderByDescending(s => s.UnitPrice) // Sắp xếp theo doanh thu giảm dần
                .ToListAsync();

            return View(statistics);
        }


        public async Task<IActionResult> ExportToExcel()
        {
            var statistics = await context.Items
                .Include(o => o.Product)
                .GroupBy(o => o.Product.Id)
                .Select(g => new
                {
                    ProductName = g.First().Product.Name,
                    Brand = g.First().Product.Brand,
                    TotalQuantity = g.Sum(o => o.Quantity),
                    TotalRevenue = g.Sum(o => o.Quantity * o.UnitPrice)
                })
                .OrderByDescending(s => s.TotalRevenue)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Revenue Statistics");

                // Header
                worksheet.Cells[1, 1].Value = "Product";
                worksheet.Cells[1, 2].Value = "Brand";
                worksheet.Cells[1, 3].Value = "Quantity";
                worksheet.Cells[1, 4].Value = "Total Revenue";

                // Dữ liệu
                int row = 2;
                foreach (var item in statistics)
                {
                    worksheet.Cells[row, 1].Value = item.ProductName;
                    worksheet.Cells[row, 2].Value = item.Brand;
                    worksheet.Cells[row, 3].Value = item.TotalQuantity;
                    worksheet.Cells[row, 4].Value = item.TotalRevenue;
                    row++;
                }

                // Định dạng bảng
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ThongKeDoanhThu.xlsx");
            }
        }


      
    }
}
