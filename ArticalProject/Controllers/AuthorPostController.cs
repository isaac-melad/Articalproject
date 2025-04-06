using ArticalProject.Core.Models;
using ArticalProject.Data.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArticalProject.Controllers
{
    [Authorize]
    public class AuthorPostController : Controller
    {
        private readonly IRepo<AuthorPost> _repo;
        private readonly IRepo<Category> _categoryRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<AuthorPostController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepo<Author> _authorrepo;
        private readonly IGetPostsByUser<AuthorPost> _getPostsByUser;
        private readonly Task<AuthorizationResult> _authorizationResult;
        

        public AuthorPostController(
            IRepo<AuthorPost> authorPostRepository,
            IRepo<Category> categoryRepository,
            IWebHostEnvironment hostEnvironment,
            ILogger<AuthorPostController> logger,
            UserManager<IdentityUser> userManager, IRepo<Author> authorrepo,
            IAuthorizationService authorizationService,
            IGetPostsByUser<AuthorPost> getPostsByUser
            )
        {
            _repo = authorPostRepository;
            _categoryRepository = categoryRepository;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _userManager = userManager;
            _authorrepo = authorrepo;
            _getPostsByUser = getPostsByUser;
            _authorizationResult = authorizationService.AuthorizeAsync(User, "Admin");
        }

        public IActionResult Index(string? searchTerm, int page = 1, int pageSize = 5)
        {
            if (_authorizationResult.Result.Succeeded)
            {
                var allAuthors = _repo.GetAll();

                // تطبيق فلتر البحث
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allAuthors = _repo.SearchByName(searchTerm);
                }

                // حساب التقسيم إلى صفحات
                int totalItems = allAuthors.Count();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var authors = allAuthors
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // تمرير بيانات التقسيم إلى العرض
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;

                return View(authors);
            }
            else
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser == null)
                {
                    return Challenge();
                }
                // إذا كان المستخدم ليس مسؤولاً، احصل على بيانات المؤلفين الخاصة به فقط
                var allAuthors = _getPostsByUser.GetPostsByUser(currentUser.Id);
                

                // تطبيق فلتر البحث
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allAuthors = _repo.SearchByName(searchTerm);
                }

                // حساب التقسيم إلى صفحات
                int totalItems = allAuthors.Count();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var authors = allAuthors
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // تمرير بيانات التقسيم إلى العرض
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;

                return View(authors);
            }
        }

        /// <summary>
        /// يبحث عن المؤلفين بواسطة الاسم
        /// </summary>
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return RedirectToAction(nameof(Index));

            var results = _repo.SearchByName(searchTerm.Trim());
            _logger.LogInformation("Search for '{SearchTerm}' returned {Count} results", searchTerm, results.Count());
            return View("Index", results);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var categoryList = _categoryRepository.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
            ViewBag.Categories = new SelectList(categoryList, "Value", "Text");
            var author = _authorrepo.GetDataByUser(currentUser.Id);

            var authorPost = new AuthorPost
            {
                UserId = currentUser.Id,
                UserName = currentUser.UserName,
                FullName = currentUser.UserName,
                AuthorId = author.Id,
            };

            return View(authorPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(AuthorPost formPost, IFormFile imageFile)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                if (ModelState.IsValid)
                {
                    string? imageUrl = string.Empty;
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                        var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "posts");
                        var path = Path.Combine(uploadsFolder, uniqueFileName);

                        Directory.CreateDirectory(uploadsFolder);
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        imageUrl = "/images/posts/" + uniqueFileName;
                    }
                    var author = _authorrepo.GetDataByUser(currentUser.Id);

                    var authorPost = new AuthorPost
                    {
                        PostTitle = formPost.PostTitle,
                        Content = formPost.Content,
                        CategoryId = formPost.CategoryId,
                        AuthorId = author.Id,
                        CreatedAt = DateTime.Now,
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName,
                        FullName = currentUser.UserName,
                        PostImageUrl = imageUrl
                    };

                    await Task.Run(() => _repo.Add(authorPost));
                    return RedirectToAction(nameof(Index));
                }

                var categoryList = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                ViewBag.Categories = new SelectList(categoryList, "Value", "Text");
                return View(formPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new post");
                ModelState.AddModelError("", "An error occurred while saving the post. Please try again.");

                var categoryList = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                ViewBag.Categories = new SelectList(categoryList, "Value", "Text");
                return View(formPost);
            }
        }
        [Authorize]

        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var authorPost = await Task.Run(() => _repo.GetById(id));
            if (authorPost == null || authorPost.UserId != currentUser.Id)
            {
                return NotFound();
            }

            var categoryList = _categoryRepository.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
            ViewBag.Categories = new SelectList(categoryList, "Value", "Text");

            var author = _authorrepo.GetDataByUser(currentUser.Id);
            authorPost.AuthorId = author.Id;

            return View(authorPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, AuthorPost formPost, IFormFile imageFile)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                if (id != formPost.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingPost = await Task.Run(() => _repo.GetById(id));
                    if (existingPost == null || existingPost.UserId != currentUser.Id)
                    {
                        return NotFound();
                    }

                    var author = _authorrepo.GetDataByUser(currentUser.Id);
                    string? imageUrl = existingPost.PostImageUrl;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingPost.PostImageUrl))
                        {
                            string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, existingPost.PostImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var fileName = Path.GetFileName(imageFile.FileName);
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                        var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "posts");
                        var path = Path.Combine(uploadsFolder, uniqueFileName);

                        Directory.CreateDirectory(uploadsFolder);
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        imageUrl = "/images/posts/" + uniqueFileName;
                    }

                    var updatedPost = new AuthorPost
                    {
                        Id = existingPost.Id,
                        PostTitle = formPost.PostTitle,
                        Content = formPost.Content,
                        CategoryId = formPost.CategoryId,
                        AuthorId = author.Id,
                        CreatedAt = existingPost.CreatedAt,
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName,
                        FullName = currentUser.UserName,
                        PostImageUrl = imageUrl
                    };

                    await Task.Run(() => _repo.Update(updatedPost));
                    return RedirectToAction(nameof(Index));
                }

                var categoryList = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                ViewBag.Categories = new SelectList(categoryList, "Value", "Text");
                return View(formPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating author post with ID: {id}");
                ModelState.AddModelError("", "An error occurred while updating the post. Please try again.");

                var categoryList = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                ViewBag.Categories = new SelectList(categoryList, "Value", "Text");
                return View(formPost);
            }
        }
        [Authorize]
        public async Task<IActionResult> GetPostsByCategory(int categoryId, int page = 1)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                const int pageSize = 10;

                var authorPosts = await Task.Run(() => _repo.GetAll());

                var filteredPosts = authorPosts
                    .Where(p => p.UserId == currentUser.Id && p.CategoryId == categoryId);

                var totalPosts = filteredPosts.Count();
                var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

                page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.CategoryId = categoryId;

                var pagedPosts = filteredPosts
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                return View("Index", pagedPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author posts for category ID: {categoryId}");
                return View("Error");
            }
        }
        
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                var authorPost = await Task.Run(() => _repo.GetById(id));
                var category= await Task.Run(() => _categoryRepository.GetById(authorPost.CategoryId));
                if (authorPost == null || authorPost.UserId != currentUser.Id)
                {
                    return NotFound();
                }
                authorPost.Category = category;
                return View(authorPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author post with ID: {id} for details view");
                return View("Error");
            }
        }
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                
                if (currentUser == null)
                {
                    return Challenge();
                }

                var authorPost = await Task.Run(() => _repo.GetById(id));
                var category = await Task.Run(() => _categoryRepository.GetById(authorPost.CategoryId));
                if (authorPost == null || authorPost.UserId != currentUser.Id)
                {
                    return NotFound();
                }
                authorPost.Category= category;
                return View(authorPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author post with ID: {id} for deletion");
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                var authorPost = await Task.Run(() => _repo.GetById(id));
                if (authorPost == null || authorPost.UserId != currentUser.Id)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(authorPost.PostImageUrl))
                {
                    string imagePath = Path.Combine(_hostEnvironment.WebRootPath, authorPost.PostImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                await Task.Run(() => _repo.Delete(id));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting author post with ID: {id}");
                return View("Error");
            }
        }
    }
}
