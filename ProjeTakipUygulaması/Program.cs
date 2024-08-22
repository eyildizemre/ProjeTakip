using Microsoft.EntityFrameworkCore;
using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.DataAccess.Repository;
using ProjeTakipUygulamasý.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ProjeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<CalendarService>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

// Enable session management
app.UseSession();

// Middleware to check if the user is logged in
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value.ToLower();

    // POST isteklerini atla
    if (!path.Contains("/account/login") &&
        string.IsNullOrEmpty(context.Session.GetString("UserId")) &&
        context.Request.Method.ToLower() != "post")
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next.Invoke();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
