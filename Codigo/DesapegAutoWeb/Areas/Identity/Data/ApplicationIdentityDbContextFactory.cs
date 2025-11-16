using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DesapegAutoWeb.Areas.Identity.Data
{
    public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
   var configuration = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json")
             .Build();

   var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
  var connectionString = configuration.GetConnectionString("DesapegAutoConnection");
    
            if (string.IsNullOrEmpty(connectionString))
        {
    throw new InvalidOperationException("Connection string 'DesapegAutoConnection' not found.");
 }
       
   optionsBuilder.UseMySQL(connectionString);

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
 }
    }
}
