using AuthenticationProject.Models;

namespace AuthenticationProject.Dtos;

public class LoginUserDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class RegisterUserDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; }
    public DateTime? ExpiredDate { get; set; }

}
public class EditRoleDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime? ExpiredDate { get; set; }
}

public class RoleDetailsDto
{
    public ApplicationRole? Role { get; set; }
    public List<ApplicationUser>? Members { get; set; }
    public List<ApplicationUser>? NonMembers { get; set; }
}