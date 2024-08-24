using Microsoft.EntityFrameworkCore;
using ProjeTakip.DataAccess.Data;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.DataAccess.Repository;
using ProjeTakipUygulaması.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ProjeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();

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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session management
app.UseSession();

// Authorization işlemleri session'dan sonra kullanılmalı.
app.UseAuthorization();

// Middleware to check if the user is logged in
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value.ToLower();

    if (!path.Contains("/account/login") &&
        string.IsNullOrEmpty(context.Session.GetString("UserId")) &&
        context.Request.Method.ToLower() != "post")
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    else if (path.Contains("/account/login") &&
             !string.IsNullOrEmpty(context.Session.GetString("UserId")) &&
             context.Request.Method.ToLower() == "post")
    {
        var roleId = context.Session.GetInt32("RoleId");

        if (roleId == 1)
        {
            context.Response.Redirect("/Admin/");
        }
        else if (roleId == 2)
        {
            context.Response.Redirect("/TeamLead/");
        }
        else if (roleId == 3)
        {
            context.Response.Redirect("/TeamMember/");
        }
        return;
    }
    await next.Invoke();
});

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
