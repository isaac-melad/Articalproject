using ArticalProject.Core.Models;
using ArticalProject.Data.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ArticalProject.Controllers
{
    [Route("[controller]")]
    [Authorize("Admin")]
    // متحكم الفئات
    public class CategoryController(IRepo<Category> categoryRepo, ILogger<CategoryController> logger) : Controller
    {
        [HttpGet]
        public IActionResult Index(string? searchTerm, int page = 1, int pageSize = 5)
        {
            var allCategories = categoryRepo.GetAll();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allCategories = categoryRepo.SearchByName(searchTerm);
            }

            // Pagination calculations
            int totalItems = allCategories.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var categories = allCategories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(categories);
        }

        [HttpGet("Create")]
        // يعرض نموذج إنشاء فئة جديدة
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        // يعالج إنشاء فئة جديدة
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    categoryRepo.Add(category);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error creating category: {Category}", category);
                    ModelState.AddModelError("", "An error occurred while creating the category.");
                }
            }
            return View(category);
        }

        [HttpGet("Edit/{id}")]
        // يعرض نموذج تعديل فئة
        public IActionResult Edit(int id)
        {
            var category = categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        // يعالج تعديل فئة
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                var existingCategory = categoryRepo.GetById(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                categoryRepo.Update(category);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating category with ID: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the category.");
                return View(category);
            }
        }

        [HttpGet("Delete/{id}")]
        // يعرض صفحة تأكيد حذف فئة
        public IActionResult Delete(int id)
        {
            var category = categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        // يؤكد حذف الفئة
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                categoryRepo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting category with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the category.");
            }
        }
    }
}
