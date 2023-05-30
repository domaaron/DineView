using DineView.Webapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DineView.Webapp.Pages.User
{
    public class LogoutModel : PageModel
    {
        private readonly AuthService _authService;

        public LogoutModel(AuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> OnGet()
        {
            await _authService.LogoutAsync();
            return Redirect("/");
        }
    }
}
