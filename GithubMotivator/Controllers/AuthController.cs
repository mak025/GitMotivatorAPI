using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.GitHub;

namespace GithubMotivator.Controllers;

[Route("[controller]")]
public class AuthController : Controller
{
    [HttpGet("Login")]
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, GitHubAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}