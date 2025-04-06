using ArticalProject.Core.Models;
using ArticalProject.Data.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ArticalProject.Controllers
{
    public class AuthorController(
        IRepo<Author> repo,
        IWebHostEnvironment hostEnvironment,
        IAuthorizationService authorService,
        ILogger<AuthorController> logger)
        : Controller
    {
        private readonly IRepo<Author> _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        private readonly ILogger<AuthorController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private const string ImageFolder = "images";
        public const int PageSize = 10;

        [Authorize("Admin")]
        public IActionResult Index(string? searchTerm, int page = 1, int pageSize = 5)
        {
            var allAuthors = _repo.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (_repo is ISearchByAnyProperties<Author> searchableRepo)
                {
                    allAuthors = searchableRepo.SearchByProperty(searchTerm);
                }
                else
                {
                    _logger.LogWarning("IRepo does not implement ISearchByAnyProperties");
                    try
                    {
                        if (_repo is RepoAuthor repoAuthor)
                        {
                            allAuthors = repoAuthor.SearchByName(searchTerm);
                        }
                        else
                        {
                            _logger.LogWarning("SearchByName method not directly available on provided IRepo instance.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during search operation.");
                    }
                }
            }

            int totalItems = allAuthors.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var authors = allAuthors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;

            return View(authors);
        }

        [Authorize("Admin")]    
        public IActionResult Search(string searchTerm)
        {
            return RedirectToAction(nameof(Index), new { searchTerm = searchTerm });
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Attempted GET edit with null ID.");
                return NotFound();
            }

            var author = _repo.GetById(id.Value);
            if (author == null)
            {
                _logger.LogWarning("Author not found for GET edit: {Id}", id);
                return NotFound();
            }
            return View(author);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,UserName,FullName,ProfileImageUrl,Bio,FacebookUrl,InstagramUrl,TwitterUrl")] Author author, IFormFile? profileImage)
        {
            if (id != author.Id)
            {
                _logger.LogWarning("Author ID mismatch in POST edit. Route ID: {RouteId}, Model ID: {ModelId}", id, author.Id);
                return NotFound();
            }

            var existingAuthor = _repo.GetById(id);
            if (existingAuthor == null)
            {
                _logger.LogWarning("Author not found for POST edit: {Id}", id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? uniqueFileName = existingAuthor.ProfileImageUrl;

                    if (profileImage != null)
                    {
                        _logger.LogInformation("New profile image uploaded for author ID: {Id}", id);
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, ImageFolder);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                            _logger.LogInformation("Created image directory: {Directory}", uploadsFolder);
                        }

                        if (!string.IsNullOrEmpty(existingAuthor.ProfileImageUrl))
                        {
                            var oldImagePath = Path.Combine(uploadsFolder, existingAuthor.ProfileImageUrl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                                _logger.LogInformation("Deleted old profile image: {ImagePath}", oldImagePath);
                            }
                        }

                        uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await profileImage.CopyToAsync(fileStream);
                        }
                        _logger.LogInformation("Saved new profile image: {FilePath}", filePath);
                        author.ProfileImageUrl = uniqueFileName;
                    }
                    else
                    {
                        author.ProfileImageUrl = existingAuthor.ProfileImageUrl;
                    }

                    existingAuthor.FullName = author.FullName;
                    existingAuthor.Bio = author.Bio;
                    existingAuthor.UserId = author.UserId;
                    existingAuthor.ProfileImageUrl = author.ProfileImageUrl;
                    existingAuthor.FacebookUrl = author.FacebookUrl;
                    existingAuthor.InstagramUrl = author.InstagramUrl;
                    existingAuthor.TwitterUrl = author.TwitterUrl;

                    _repo.Update(existingAuthor);
                    _logger.LogInformation("Successfully updated author: {Id}", id);
                    TempData["SuccessMessage"] = "Author successfully updated.";
                    var isAuthorized = authorService.AuthorizeAsync(User, "Admin");
                    if (isAuthorized.Result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), "Admin");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating author: {Id}", id);
                    ModelState.AddModelError("", "An error occurred while updating the author. Please try again.");
                }
            }
            else
            {
                _logger.LogWarning("Model state invalid for POST edit, author ID: {Id}", id);
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    if (value != null)
                    {
                        foreach (var error in value.Errors)
                        {
                            _logger.LogWarning("ModelState Error for {Key}: {ErrorMessage}", modelStateKey, error.ErrorMessage);
                        }
                    }
                }
            }

            author.ProfileImageUrl = existingAuthor?.ProfileImageUrl ?? author.ProfileImageUrl;
            return View(author);
        }

        [Authorize("Admin")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Attempted to view details with null ID");
                return NotFound();
            }

            var author = _repo.GetById(id.Value);
            if (author == null)
            {
                _logger.LogWarning("Author not found for details: {Id}", id);
                return NotFound();
            }

            return View(author);
        }

        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Attempted deletion with null ID");
                return NotFound();
            }

            var author = _repo.GetById(id.Value);
            if (author == null)
            {
                _logger.LogWarning("Author not found for deletion: {Id}", id);
                return NotFound();
            }
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var author = _repo.GetById(id);
                if (author == null)
                {
                    _logger.LogWarning("Author not found during deletion: {Id}", id);
                    return NotFound();
                }

                var imagePath = string.IsNullOrEmpty(author.ProfileImageUrl)
                    ? null
                    : Path.Combine(_hostEnvironment.WebRootPath, ImageFolder, author.ProfileImageUrl);

                if (imagePath != null && System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                        _logger.LogInformation("Deleted profile image during author deletion: {ImagePath}", imagePath);
                    }
                    catch (IOException ioEx)
                    {
                        _logger.LogError(ioEx, "Error deleting image file {ImagePath} for author {Id}", imagePath, id);
                        TempData["ErrorMessage"] = "Could not delete associated image file, but author record will be removed.";
                    }
                }

                _repo.Delete(id);
                _logger.LogInformation("Successfully deleted author: {Id}", id);
                TempData["SuccessMessage"] = "Author successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting author: {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the author.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
