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

            // Register DbContext (in-memory for simple setup)
            builder.Services.AddDbContext<DesapegAutoContext>(options =>
                options.UseInMemoryDatabase("DesapegAuto"));

            var connectionString = builder.Configuration.GetConnectionString("DesapegAutoConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DesapegAutoConnection' not found.");
            }

            // Configure Identity DbContext with MySQL
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseMySQL(connectionString));

            // Configure Identity with Roles
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    // Configurações opcionais de senha
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            // Register application services
            builder.Services.AddScoped<IVeiculoService, VeiculoService>();
            builder.Services.AddScoped<IMarcaService, MarcaService>();
            builder.Services.AddScoped<IVendaService, VendaService>();
            builder.Services.AddScoped<IVersaoService, VersaoService>();
            builder.Services.AddScoped<IConcessionariaService, ConcessionariaService>();
            builder.Services.AddScoped<IPessoaService, PessoaService>();
            builder.Services.AddScoped<IModeloService, ModeloService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IAnuncioService, AnuncioService>();

            // Register AutoMapper scanning current assembly
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Inicializar banco de dados e criar roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    // Garante que o banco de dados existe e aplica migrations pendentes
                    var dbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
                    dbContext.Database.EnsureCreated();
                    logger.LogInformation("Banco de dados verificado/criado com sucesso.");
                    
                    // Cria as roles padrão
                    SeedRoles(services).Wait();
                    logger.LogInformation("Roles foram criadas com sucesso.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Não foi possível inicializar o banco de dados ou criar as roles. " +
                        "Verifique a conexão com o MySQL e a string de conexão no arquivo appsettings.json.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
