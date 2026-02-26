using Microsoft.EntityFrameworkCore;
using Core;
using Core.Service;
using Service;
using Microsoft.AspNetCore.Identity;
using DesapegAutoWeb.Areas.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DesapegAutoWeb
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var connectionString = builder.Configuration.GetConnectionString("DesapegAutoConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DesapegAutoConnection' not found.");
            }

            // Main application context now uses MySQL (persistent storage).
            builder.Services.AddDbContext<DesapegAutoContext>(options =>
                options.UseMySQL(connectionString));

            // Identity context also uses MySQL.
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseMySQL(connectionString));

            // Configure Identity with roles.
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            // Register application services.
            builder.Services.AddScoped<IVeiculoService, VeiculoService>();
            builder.Services.AddScoped<IMarcaService, MarcaService>();
            builder.Services.AddScoped<IVendaService, VendaService>();
            builder.Services.AddScoped<IVersaoService, VersaoService>();
            builder.Services.AddScoped<IConcessionariaService, ConcessionariaService>();
            builder.Services.AddScoped<IPessoaService, PessoaService>();
            builder.Services.AddScoped<IModeloService, ModeloService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IAnuncioService, AnuncioService>();

            // Register AutoMapper scanning current assembly.
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Initialize databases and roles.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    // Apply Identity migrations to ensure tables exist even when DB already exists.
                    var identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
                    EnsureIdentityMigrationBaseline(identityDbContext);
                    identityDbContext.Database.Migrate();
                    logger.LogInformation("Banco de identidade migrado com sucesso.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Migracao de identidade falhou (as tabelas podem ja existir). Continuando...");
                }

                try
                {
                    // Create default roles (runs even if migration was skipped).
                    SeedRoles(services).Wait();
                    logger.LogInformation("Roles foram criadas com sucesso.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Nao foi possivel criar as roles. " +
                        "Verifique a conexao com o MySQL e a string de conexao no arquivo appsettings.json.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        private static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Funcionario", "Cliente" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static void EnsureIdentityMigrationBaseline(ApplicationIdentityDbContext identityDbContext)
        {
            const string migrationId = "20251115235408_CreateIdentitySchema";
            const string productVersion = "8.0.18";

            identityDbContext.Database.ExecuteSqlRaw($@"
                INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
                SELECT {{0}}, {{1}}
                FROM DUAL
                WHERE EXISTS (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND LOWER(TABLE_NAME) = 'aspnetroles'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM __EFMigrationsHistory
                    WHERE MigrationId = {{0}}
                );", migrationId, productVersion);
        }

        
    }
}