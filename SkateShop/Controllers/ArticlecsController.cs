using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkateShop.Models;
using SkateShop.Services;

namespace SkateShop.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ArticlecsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5;

        public ArticlecsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index(int pageIndex, string? search)
        {
            IQueryable<Articlecs> query = context.Articlecs;

            //tìm kiếm
            if (search != null)
            {
                query = query.Where(p => p.Title.Contains(search));
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
        public IActionResult Create(ArticlecsDto articlecsDto)
        {
            if (articlecsDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(articlecsDto);
            }


            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(articlecsDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/articlecs/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                articlecsDto.ImageFile.CopyTo(stream);
            }

            Articlecs articlecs = new Articlecs()
            {
                Title = articlecsDto.Title,
                Content = articlecsDto.Content,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };


            context.Articlecs.Add(articlecs);
            context.SaveChanges();

            return RedirectToAction("Index", "Articlecs");
        }

        public IActionResult Edit(int id)
        {
            var a = context.Articlecs.Find(id);

            if (a == null)
            {
                return RedirectToAction("Index", "Articlecs");
            }

            var articlecsDto = new ArticlecsDto()
            {
                Title=a.Title,
                Content = a.Content,
            };


            ViewData["ArticlecId"] = a.Id;
            ViewData["ImageFileName"] = a.ImageFileName;
            ViewData["CreatedAt"] = a.CreatedAt.ToString("MM/dd/yyyy");

            return View(articlecsDto);
        }


        [HttpPost]
        public IActionResult Edit(int id, ArticlecsDto articlecsDto)
        {
            var a = context.Articlecs.Find(id);

            if (a == null)
            {
                return RedirectToAction("Index", "Articlecs");
            }


            if (!ModelState.IsValid)
            {
                ViewData["ArticlecId"] = a.Id;
                ViewData["ImageFileName"] = a.ImageFileName;
                ViewData["CreatedAt"] = a.CreatedAt.ToString("MM/dd/yyyy");

                return View(articlecsDto);
            }

            string newFileName = a.ImageFileName;
            if (articlecsDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(articlecsDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/articlecs/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    articlecsDto.ImageFile.CopyTo(stream);
                }

                // xóa ảnh cũ
                string oldImageFullPath = environment.WebRootPath + "/articlecs/" + a.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }


            a.Title = articlecsDto.Title;
            a.Content = articlecsDto.Content;
            a.ImageFileName = newFileName;


            context.SaveChanges();

            return RedirectToAction("Index", "Articlecs");
        }

        public IActionResult Delete(int id)
        {
            var a = context.Articlecs.Find(id);
            if (a == null)
            {
                return RedirectToAction("Index", "Articlecs");
            }

            string imageFullPath = environment.WebRootPath + "/articlecs/" + a.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Articlecs.Remove(a);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Articlecs");
        }


    }
}

