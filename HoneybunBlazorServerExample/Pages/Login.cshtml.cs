using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoneybunBlazorServerExample.Pages
{
    public class LoginModel : PageModel
    {
        // *** START INTEGRATING HONEYBUN AUTH
        public async Task OnGet(string redirectUri)
        {
            await HttpContext.ChallengeAsync(
                OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = redirectUri });
        }
        // *** END INTEGRATING HONEYBUN AUTH
    }
}
