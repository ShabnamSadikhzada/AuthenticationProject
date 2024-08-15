using AuthenticationProject.Dtos;
using AuthenticationProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationProject.Controllers;

public class UsersController : Controller
{
    #region Constructor
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        IPasswordValidator<ApplicationUser> passwordValidator,
        RoleManager<ApplicationRole> roleManager)
    {
        this._userManager = userManager;
        this._passwordValidator = passwordValidator;
        this._roleManager = roleManager;
    }

    #endregion

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(model: users);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create([Bind("UserName, Email, Password")] RegisterUserDto model)
    {
        if(ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                string defaultRole = "User";
                if(!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = defaultRole });
                }

                await _userManager.AddToRoleAsync(user, defaultRole);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        return View(model);
    }

}
