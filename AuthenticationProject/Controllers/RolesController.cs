using AuthenticationProject.Dtos;
using AuthenticationProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AuthenticationProject.Controllers;

public class RolesController : Controller
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index() => View(await _roleManager.Roles.ToListAsync());

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDto model)
    {
        if (ModelState.IsValid)
        {
            var role = new ApplicationRole
            {
                Name = model.Name,
                ExpiredDate = model.ExpiredDate
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
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


    public async Task<IActionResult> Edit(string id)
    {
        ApplicationRole role = await _roleManager.FindByIdAsync(id);
        var members = new List<ApplicationUser>();
        var nonMembers = new List<ApplicationUser>();


        foreach (var user in _userManager.Users)
        {
            var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
            list.Add(user);
        }


        var dto = new RoleDetailsDto
        {
            Role = role,
            Members = members,
            NonMembers = nonMembers
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([FromBody] UserRoleEditDto dto)
    {
        ApplicationRole _role = await _roleManager.FindByIdAsync(dto.RoleId);
        IdentityResult result = new();

        if(ModelState.IsValid)
        {
            foreach (string mail in dto.Emails)
            {
                var user = await _userManager.FindByEmailAsync(mail);

                if (user != null)
                {
                    bool isInRole = await _userManager.IsInRoleAsync(user, _role.Name);

                    if(isInRole)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, _role.Name);
                    }
                    else
                    {
                        result = await _userManager.AddToRoleAsync(user, _role.Name);
                    }
                }
            }
        }


        return Json(new
        {
            StatusCode = result.Succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
            Messages = result.Succeeded ? new[] { "Success" } : result.Errors.Select(x => x.Description).ToArray()
        });
    }
}
