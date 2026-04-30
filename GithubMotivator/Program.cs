using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OAuth.GitHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["GitHub:ClientId"] ?? throw new InvalidOperationException("GitHub ClientId is missing.");
    options.ClientSecret = builder.Configuration["GitHub:ClientSecret"] ?? throw new InvalidOperationException("GitHub ClientSecret is missing.");
    options.CallbackPath = "/signin-github";

    options.Scope.Add("user:email");
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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();