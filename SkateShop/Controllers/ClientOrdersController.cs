using SkateShop.Models;
using SkateShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SkateShop.Controllers
{
    [Authorize(Roles = "client")]
    [Route("/Client/Orders/{action=Index}/{id?}")]
    public class ClientOrdersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly int pageSize = 5;

        public ClientOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageIndex)
        {

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Order> query = context.Orders.Include(o => o.Items).OrderByDescending(o => o.Id).Where(o => o.ClientId == currentUser.Id);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }


            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var orders = query.ToList();

            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = context.Orders.Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.ClientId == currentUser.Id)
                .FirstOrDefault(o => o.Id == id);


            if (order == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.NumOrders = context.Orders.Where(o => o.ClientId == order.ClientId).Count();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index");
            }

            var order = await context.Orders
                .Include(o => o.Items) // Tải danh sách OrderItems
                .Where(o => o.ClientId == currentUser.Id)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            // Kiểm tra trạng thái đơn hàng
            if (order.OrderStatus == "shipped" || order.OrderStatus == "delivered")
            {
                TempData["ErrorMessageClientOrder"] = "You cannot Cancel orders that are in 'shipped' or 'delivered' status.";
                return RedirectToAction("Index");
            }

            // Xóa tất cả OrderItems trước khi xóa Order
            context.RemoveRange(order.Items);

            // Xóa Order
            context.Orders.Remove(order);

            await context.SaveChangesAsync();

            TempData["SuccessMessageClientOrder"] = "Order deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}
