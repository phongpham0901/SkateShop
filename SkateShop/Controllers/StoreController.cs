using Microsoft.AspNetCore.Mvc;
using SkateShop.Models;
using SkateShop.Services;

namespace SkateShop.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;

        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(int pageIndex, string? search, string? name, string? brand, string? sort)
        {
            IQueryable<Product> query = context.Products;

            //Tìm kiếm
            if (search != null && search.Length > 0)
            {
                query = query.Where(p => p.Name.Contains(search));
            }


            // Lọc
            if (brand != null && brand.Length > 0)
            {
                query = query.Where(p => p.Brand.Contains(brand));
            }

            

            // Sắp xếp
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else if (sort == "oldest")
            {
                query = query.OrderBy(p => p.Id);
            }
            else
            {
                query = query.OrderByDescending(p => p.Id);
            }


            // phân trang
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var products = query.ToList();

            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Name = name,
                Brand = brand,
                Sort = sort
            };

            return View(storeSearchModel);
        }

        public IActionResult Sale()
        {
            var query = context.Products.ToList();

            return View(query);
        }

        public IActionResult Details(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Store");
            }

            // Lấy danh sách đánh giá cho sản phẩm
            var assessments = context.Assesses.Where(a => a.Product.Id == id).ToList();

            ViewBag.Assessments = assessments; // Truyền danh sách qua ViewBag

            // Truyền sản phẩm qua ViewBag và danh sách qua Model
            ViewBag.Assess = product;


            return View(product);
        }

        [HttpGet]
        public IActionResult AddAssess(int id)
        {
            // Kiểm tra xem ID có được truyền không
            Console.WriteLine("Product ID: " + id);

            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction("Index", "Store");
            }

            ViewBag.AddAssess = product; // Truyền sản phẩm vào ViewBag
            return View();
        }

        [HttpPost]
        public IActionResult AddAssess(AssessDto assessDto)
        {
            // Tìm sản phẩm theo ProductId
            var product = context.Products.FirstOrDefault(p => p.Id == assessDto.ProductId);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm thì trả về 404
            }

            // Tạo đối tượng Assess
            Assess a = new Assess()
            {
                start = assessDto.start,
                Name = assessDto.Name,
                Description = assessDto.Description,
                Product = product // Gán toàn bộ đối tượng Product
            };

            // Lưu vào database
            context.Add(a);
            context.SaveChanges();

            // Quay lại trang Assess của sản phẩm đó
            return RedirectToAction("Details", "Store", new { id = assessDto.ProductId });
        }
    }
}
