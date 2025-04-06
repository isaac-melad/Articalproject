using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ArticalProject.Data.Repo;
using ArticalProject.Core.Models;
using Microsoft.AspNetCore.Identity;
namespace ArticalProject.Controllers
{
    [Authorize]
    public class AdminController(IRepo<AuthorPost> repo, UserManager<IdentityUser> userManager)
        : Controller
    {
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            ViewBag.PostCount = repo.GetAll().Count(x => x.UserId == currentUser.Id);
            ViewBag.PostCountThisYear = repo.GetAll().Count(x => x.UserId == currentUser.Id && x.CreatedAt.Year == DateTime.Now.Year);
            ViewBag.PostCountThisMonth = repo.GetAll().Count(x => x.UserId == currentUser.Id && x.CreatedAt.Month == DateTime.Now.Month);

            var author = repo.GetDataByUser(currentUser.Id);

            // Get the author ID for the profile edit link
            try
            {
                // Since we already have the author object, we can use its Id
                ViewBag.AuthorId = author.Id;
            }
            catch (Exception)
            {
                // If author ID can't be found, we'll handle it in the view
                ViewBag.AuthorId = 0;
            }

            return View(author);
        }
    }
}