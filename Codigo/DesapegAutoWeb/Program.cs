using Microsoft.EntityFrameworkCore;
using Core;
using Core.Service;
using Service;
using Microsoft.AspNetCore.Identity;
using DesapegAutoWeb.Areas.Identity.Data;
using DesapegAutoWeb.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DesapegAutoConnection") ?? throw new InvalidOperationException("Connection string 'DesapegAutoConnection' not found.");

builder.Services.AddDbContext<DesapegAutoContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

builder.Services.AddScoped<IAnuncioService, AnuncioService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IModeloService, ModeloService>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IConcessionariaService, ConcessionariaService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<IVersaoService, VersaoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var identityContext = services.GetRequiredService<ApplicationIdentityDbContext>();
        identityContext.Database.Migrate();

        var context = services.GetRequiredService<DesapegAutoContext>();
        DbInitializer.Initialize(context);

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await IdentityInitializer.Initialize(roleManager, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
