using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkateShop.Models;
using SkateShop.Services;

namespace SkateShop.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 5;

        public CategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(int pageIndex, string? search)
        {
            IQueryable<Category> query = context.categories;

            //tìm kiếm
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search));
            }


            //phân trang

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }

            // Kiểm tra xem danh mục đã tồn tại chưa
            bool isDuplicate = context.categories.Any(c => c.Name.ToLower() == categoryDto.Name);
            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "Category name already exists.");
                return View(categoryDto);
            }

            // Thêm mới nếu không trùng
            Category category = new Category()
            {
                Name = categoryDto.Name,
            };

            context.categories.Add(category);
            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }


        public IActionResult Edit(int id)
        {
            var a = context.categories.Find(id);

            if (a == null)
            {
                return RedirectToAction("Index", "Category");
            }

            var categoryDto = new CategoryDto()
            {
                Name=a.Name,
            };


            ViewData["CategoryId"] = a.Id;

            return View(categoryDto);
        }


        [HttpPost]
        public IActionResult Edit(int id, CategoryDto categoryDto)
        {
            var a = context.categories.Find(id);

            if (a == null)
            {
                return RedirectToAction("Index", "Category");
            }


            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = a.Id;

                return View(categoryDto);
            }

           

            a.Name = categoryDto.Name;


            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        public IActionResult Delete(int id)
        {
            var a = context.categories.Find(id);
            if (a == null)
            {
                return RedirectToAction("Index", "Category");
            }

            context.categories.Remove(a);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Category");
        }


    }
}

