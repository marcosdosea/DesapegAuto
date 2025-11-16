using DesapegAutoWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DesapegAutoWeb.Areas.Identity.Data
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
 : base(options)
   {
     }

        protected override void OnModelCreating(ModelBuilder builder)
        {
 base.OnModelCreating(builder);
            // Customizações adicionais aqui, se necessário
        }
    }
}
