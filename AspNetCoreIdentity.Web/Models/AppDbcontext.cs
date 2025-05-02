using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Models
{
    public class AppDbcontext :  IdentityDbContext<AppUser,AppRole,string>
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options): base(options){}

    }
}
