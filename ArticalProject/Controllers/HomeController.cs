using ArticalProject.Core.Models;
using ArticalProject.Data.Repo;
using ArticalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace ArticalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepo<AuthorPost> _repo;
        private readonly IRepo<Category> _categoryRepo;

        public HomeController(ILogger<HomeController> logger, IRepo<AuthorPost> repo, IRepo<Category> categoryRepo)
        {
            _logger = logger;
            _repo = repo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Details(int id)
        {
            var post = _repo.GetById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        public IActionResult Index(string? searchTerm, string? category, int page = 1, int pageSize = 5)
        {
            var allPosts = _repo.GetAll();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allPosts = _repo.SearchByName(searchTerm);
            }

            // Apply category filter if provided
            if (!string.IsNullOrEmpty(category))
            {
                allPosts = allPosts.Where(p => p.Category != null && p.Category.Name == category).ToList();
            }

            // Get all categories for the filter dropdown
            var categories = _categoryRepo.GetAll().Select(c => c.Name).ToList();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.SearchTerm = searchTerm;

            // Pagination
            var authorPosts = allPosts.ToList();
            int totalItems = authorPosts.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var posts = authorPosts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(posts);
        }
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return RedirectToAction(nameof(Index));

            var results = _repo.SearchByName(searchTerm.Trim());
            _logger.LogInformation("Search for '{SearchTerm}' returned {Count} results", searchTerm, results.Count());

            // Get all categories for the filter dropdown
            var categories = _categoryRepo.GetAll().Select(c => c.Name).ToList();
            ViewBag.Categories = categories;
            ViewBag.SearchTerm = searchTerm;

            return View("Index", results);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Publishers()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Categories()
        {
            var categories = _categoryRepo.GetAll().ToList();
            return View(categories);
        }

        public IActionResult PostsByCategory(int categoryId)
        {
            var category = _categoryRepo.GetById(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            var posts = _repo.GetAll().Where(p => p.CategoryId == categoryId).ToList();
            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = categoryId;

            return View(posts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}