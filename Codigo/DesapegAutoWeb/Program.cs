using Microsoft.EntityFrameworkCore;
using Core;
using Core.Service;
using Service;
using Microsoft.AspNetCore.Identity;
using DesapegAutoWeb.Areas.Identity.Data;

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
                    // Ensure main schema exists.
                    var appDbContext = services.GetRequiredService<DesapegAutoContext>();
                    appDbContext.Database.EnsureCreated();
                    logger.LogInformation("Banco principal verificado/criado com sucesso.");

                    // Ensure identity schema exists.
                    var identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
                    identityDbContext.Database.EnsureCreated();
                    logger.LogInformation("Banco de identidade verificado/criado com sucesso.");

                    // Create default roles.
                    SeedRoles(services).Wait();
                    logger.LogInformation("Roles foram criadas com sucesso.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Nao foi possivel inicializar o banco de dados ou criar as roles. " +
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
    }
}