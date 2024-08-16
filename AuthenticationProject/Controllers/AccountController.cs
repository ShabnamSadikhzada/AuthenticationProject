using AuthenticationProject.Dtos;
using AuthenticationProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationProject.Controllers;


//[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //[AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto model, [FromQuery] string? ReturnUrl)
    {
        if(ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    string decodeUrl = Uri.UnescapeDataString(ReturnUrl ?? @"\");

                    return Redirect(decodeUrl);
                }
            }

            ModelState.AddModelError("username", "invalid username or password");
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
