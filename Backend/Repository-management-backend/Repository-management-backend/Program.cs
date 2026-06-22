using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Repository_management_backend.Data;
using Repository_management_backend.Mapping;
using Repository_management_backend.Repositories;
using Repository_management_backend.Security;
using Repository_management_backend.Services;
using Repository_management_backend.Validators;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// MVC + JSON (enum-ları mətn kimi qəbul et/qaytar)
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// AutoMapper (v15 — cfg parametri məcburidir. Pulsuz Community tier istifadə olunur;
// log xəbərdarlığını susdurmaq üçün automapper.io-dan pulsuz açar alıb cfg.LicenseKey-ə yaza bilərsiniz)
builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);

// FluentValidation (avtomatik validation + validator-ları registrasiya et)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// Repository + Service qatları
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// HttpContext (claims oxumaq + DbContext branch filter üçün)
builder.Services.AddHttpContextAccessor();

// EF Core — SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Security servisləri
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Authentication — Cookie
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Authorization — rol əsaslı policy-lər
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageUsers", p => p.RequireRole("Admin"));
    options.AddPolicy("CanCreate", p => p.RequireRole("Admin", "Manager"));
    options.AddPolicy("CanEdit", p => p.RequireRole("Admin", "Manager"));
    options.AddPolicy("CanDelete", p => p.RequireRole("Admin", "Manager"));
});

var app = builder.Build();

// Migrasiyaları tətbiq et + demo işçiləri seed et
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    DbInitializer.Seed(db, hasher);
}

// HTTP pipeline
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

// Tətbiq Login səhifəsindən başlayır. Dashboard: /Home/Index, Yeni qaimə: /Invoice/Create
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
