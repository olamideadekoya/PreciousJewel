using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiLearning.Web.Areas.Identity.Data;
using WebApiLearning.Web.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebApiLearningWebContextConnection") ?? throw new InvalidOperationException("Connection string 'WebApiLearningWebContextConnection' not found.");

builder.Services.AddDbContext<WebApiLearningWebContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<WebApiLearningWebUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<WebApiLearningWebContext>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

app.Run();
