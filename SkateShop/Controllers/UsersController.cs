using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkateShop.Models;
using SkateShop.Services;
using Microsoft.EntityFrameworkCore;

namespace SkateShop.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly int pageSize = 5;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public IActionResult Index(int? pageIndex, string? search)
        {
            IQueryable<ApplicationUser> query = userManager.Users.OrderByDescending(u => u.CreatedAt);

            //tìm kiếm
            if (search != null)
            {
                query = query.Where(p => p.PhoneNumber.Contains(search));
            }

            // phân trang
            if (pageIndex == null || pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip(((int)pageIndex - 1) * pageSize).Take(pageSize);

            var users = query.ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;
            ViewData["Search"] = search ?? "";
            return View(users);
        }


        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }

            ViewBag.Roles = await userManager.GetRolesAsync(appUser);

            // lấy vai trò
            var availableRoles = roleManager.Roles.ToList();
            var items = new List<SelectListItem>();
            foreach (var role in availableRoles)
            {
                items.Add(
                    new SelectListItem
                    {
                        Text = role.NormalizedName,
                        Value = role.Name,
                        Selected = await userManager.IsInRoleAsync(appUser, role.Name!),
                    });
            }

            ViewBag.SelectItems = items;

            return View(appUser);
        }


        public async Task<IActionResult> EditRole(string? id, string? newRole)
        {
            if (id == null || newRole == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var roleExists = await roleManager.RoleExistsAsync(newRole);
            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null || !roleExists)
            {
                return RedirectToAction("Index", "Users");
            }

			var currentUser = await userManager.GetUserAsync(User);
			if (currentUser!.Id == appUser.Id)
			{
				TempData["ErrorMessage"] = "You cannot update your own role!";
				return RedirectToAction("Details", "Users", new { id });
			}

			if (appUser.LastName == "Admin")
            {
                TempData["ErrorMessageIndex"] = "You cannot update Admin role!";
                return RedirectToAction("Index", "Users");
            }

			// cập nhật vai trò người dùng
			var userRoles = await userManager.GetRolesAsync(appUser);
            await userManager.RemoveFromRolesAsync(appUser, userRoles);
            await userManager.AddToRoleAsync(appUser, newRole);

            TempData["SuccessMessage"] = "User Role updated successfully";
            return RedirectToAction("Details", "Users", new { id });
        }



        public async Task<IActionResult> DeleteAccount(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }

			var currentUser = await userManager.GetUserAsync(User);
			if (currentUser!.Id == appUser.Id)
			{
				TempData["ErrorMessage"] = "You cannot delete your own account!";
				return RedirectToAction("Details", "Users", new { id });
			}

			if (appUser.LastName == "Admin")
			{
				TempData["ErrorMessageIndex"] = "You cannot delete Admin role!";
				return RedirectToAction("Index", "Users");
			}

            //Check if the user has existing orders before allowing deletion
            bool hasOrders = await context.Orders.AnyAsync(o => o.ClientId == appUser.Id);
            if (hasOrders)
            {
                TempData["ErrorMessage"] = "Cannot delete this user because they have existing orders.";
                return RedirectToAction("Details", "Users", new { id });
            }

            // xóa
            var result = await userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users");
            }

            TempData["ErrorMessage"] = "Unable to delete this account: " + result.Errors.First().Description;
            return RedirectToAction("Details", "Users", new { id });
        }
    }
}
