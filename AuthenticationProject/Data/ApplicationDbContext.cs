using AuthenticationProject.Cryptography;
using AuthenticationProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationProject.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<Tenant> Tenants { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

}

public class ApplicationDbContextFactory
{
    private readonly IHttpContextAccessor _httpcontextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ICryptographyService _cryptographyService;
    private readonly ApplicationDbContext _applicationDbContext;

    public ApplicationDbContextFactory(
        IHttpContextAccessor httpcontextAccessor, 
        IConfiguration configuration, 
        ICryptographyService cryptographyService, 
        ApplicationDbContext applicationDbContext)
    {
        _httpcontextAccessor = httpcontextAccessor;
        _configuration = configuration;
        _cryptographyService = cryptographyService;
        _applicationDbContext = applicationDbContext;
    }

    public ApplicationDbContext CreateContext()
    {
        var tenantID = _httpcontextAccessor.HttpContext!.Request.Headers["TenantId"].FirstOrDefault();

        var user = _httpcontextAccessor.HttpContext.User;

        string connectionString = default;
        
        if(string.IsNullOrEmpty(tenantID))
        {
            connectionString = _configuration.GetConnectionString("default");
        }
        else
        {
            var tenant = _applicationDbContext.Tenants.FirstOrDefault(x => x.TenantId == int.Parse(tenantID));
            connectionString = _cryptographyService.Decrypt(tenant.ConnectionString, tenant.TenancyName);
        }

        var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();  
        optionBuilder.UseSqlServer(connectionString);
        return new ApplicationDbContext(optionBuilder.Options);
    }
}
