using electrible.Context;
using electrible.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace electrible.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                return View(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Shop()
        {
            var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            ViewBag.Categories = categories;

            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public JsonResult GetProducts(string searchTerm = "", string category = "", string priceRange = "")
        {
            IQueryable<Product> query = _context.Products;

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceRanges = priceRange.Split('-').Select(int.Parse).ToArray();
                if (priceRanges.Length == 2)
                {
                    query = query.Where(p => p.Price >= priceRanges[0] && p.Price <= priceRanges[1]);
                }
            }

            // Select and project the results
            var products = query.Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Description,
                p.Price,
                p.Quantity,
                p.ImageUrl,
                Category = p.Category.Name
            }).ToList();

            return Json(products);
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form, IFormFile imageFile)
        {
            var product = new Product
            {
                Name = form["Name"],
                Description = form["Description"],
                Price = decimal.Parse(form["Price"]),
                Quantity = int.Parse(form["Quantity"]),
                CategoryId = int.Parse(form["CategoryId"])
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                var imagePath = Path.Combine("wwwroot/images", imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/" + imageFile.FileName;
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormCollection form, IFormFile imageFile = null, string existingImageUrl = null)
        {
            var product = await _context.Products.FindAsync(int.Parse(form["ProductId"]));
            if (product == null)
            {
                return NotFound();
            }

            product.Name = form["Name"];
            product.Description = form["Description"];
            product.Price = decimal.Parse(form["Price"]);
            product.Quantity = int.Parse(form["Quantity"]);
            product.CategoryId = int.Parse(form["CategoryId"]);

            // Check if a new image file is uploaded
            if (imageFile != null && imageFile.Length > 0)
            {
                // If a new image file is uploaded, update the ImageUrl property
                var imagePath = Path.Combine("wwwroot/images", imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/" + imageFile.FileName;
            }
            else
            {
                // If no new image file is uploaded, retain the existing image URL
                product.ImageUrl = existingImageUrl;
            }

            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

    }
}
