using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerLedgerRepository, CustomerLedgerRepository>();
builder.Services.AddScoped<ICustomerLedgerService, CustomerLedgerService>();
builder.Services.AddScoped<IExtensionRepository, ExtensionRepository>();
builder.Services.AddScoped<IExtensionService, ExtensionService>();
builder.Services.AddScoped<IReturnRepository, ReturnRepository>();
builder.Services.AddScoped<IReturnService, ReturnService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

// HttpContext (claims oxumaq + DbContext branch filter üçün)
builder.Services.AddHttpContextAccessor();

// EF Core — SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        // Invoice query filter + məcburi uşaq naviqasiyaları üçün benign xəbərdarlığı susdur
        .ConfigureWarnings(w =>
            w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)));

// Security servisləri
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Authentication — Cookie (production sərtləşdirməsi)
var isProd = builder.Environment.IsProduction();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "Kapital.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = isProd ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
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
    var sp = scope.ServiceProvider;
    try
    {
        var db = sp.GetRequiredService<AppDbContext>();
        var hasher = sp.GetRequiredService<IPasswordHasher>();
        DbInitializer.Seed(db, hasher);
    }
    catch (Exception ex)
    {
        sp.GetRequiredService<ILogger<Program>>()
          .LogError(ex, "Migrasiya/seed zamanı xəta baş verdi.");
        throw; // startup-da görünməsi üçün yenidən atılır
    }
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

// API controller-ləri (attribute route) açıq şəkildə map et
app.MapControllers();

// MVC səhifələri. Tətbiq Login-dən başlayır. Dashboard: /Home/Index, Yeni qaimə: /Invoice/Create
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
