using ArticalProject.Core.Models;
using ArticalProject.Data;
using ArticalProject.Data.DataEf;
using ArticalProject.Data.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<DataContextEf>(options => options.UseSqlServer(connectionString));
//builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<IRepo<Category>, RepoCategory>();
builder.Services.AddTransient<IRepo<Author>, RepoAuthor>();
builder.Services.AddTransient<IRepo<AuthorPost>, RepoAuthorPost>();
builder.Services.AddTransient<IGetPostsByUser<AuthorPost>, RepoAuthorPost>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("User", p => p.RequireClaim("Uer", "Uer"));
    option.AddPolicy("Admin", p => p.RequireClaim("Admin", "Admin"));
});
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
