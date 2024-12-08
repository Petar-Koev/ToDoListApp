using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Models;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Controllers
{
    public class AchievementController : Controller
    {
        private readonly IAchievementService _service;
        private readonly UserManager<IdentityUser> _userManager;

        public AchievementController(IAchievementService service, UserManager<IdentityUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;
            var userAchievements = await _service.GetUserAchievementsAsync(userId);
            var availableAchievements = await _service.GetAvailableAchievementsAsync(userId);

            var model = new AchievementViewModel
            {
                Achieved = userAchievements,
                Available = availableAchievements
            };

            return View(model);
        }
    }
}
