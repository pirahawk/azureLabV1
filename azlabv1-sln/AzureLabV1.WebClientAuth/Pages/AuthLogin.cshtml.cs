using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureLabV1.WebClientAuth.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken] // You need to set this or get 400 all the time because of CSRF token missing
    public class AuthLoginModel : PageModel
    {
        public void OnGet()
        {
        }

        
        public void OnPost()
        {
            var user = this.User;
        }
    }
}
