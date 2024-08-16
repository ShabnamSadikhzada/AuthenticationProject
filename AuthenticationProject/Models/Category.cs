namespace AuthenticationProject.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class Tenant
{
    public int TenantId { get; set; }
    public string Name { get; set; }
    public string TenancyName 
    {
        get => this.Name
            .ToLower()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "");            
    }
    public string? ConnectionString { get; set; }

}
