using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OAuth.GitHub;
using GithubMotivator.Data;
using GithubMotivator.Models;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    options.SaveTokens = true;

    options.Events.OnCreatingTicket = async context =>
    {
        var username = context.Identity?.FindFirst(ClaimTypes.Name)?.Value;
        var email = context.Identity?.FindFirst(ClaimTypes.Email)?.Value;
        var accessToken = context.AccessToken;

        if (string.IsNullOrEmpty(username)) return;

        // Fetch commits from GitHub using HttpClient
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GithubMotivator");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        int commitCount = 0;
        try
        {
            var response = await httpClient.GetAsync($"https://api.github.com/search/commits?q=author:{username}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                commitCount = doc.RootElement.GetProperty("total_count").GetInt32();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching commits: {ex.Message}");
        }

        // Save to DB
        using var scope = context.HttpContext.RequestServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            user = new GithubMotivator.Models.User
            {
                Username = username,
                Email = email ?? "",
                Name = context.Identity?.FindFirst("urn:github:name")?.Value ?? username,
                Commits = commitCount,
                Type = GithubMotivator.Models.User.UserType.Contributor // Default
            };
            dbContext.Users.Add(user);
        }
        else
        {
            user.Commits = commitCount;
            if (!string.IsNullOrEmpty(email)) user.Email = email;
            dbContext.Users.Update(user);
        }

        await dbContext.SaveChangesAsync();
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure Database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
        name: "api",
        pattern: "api/{controller}/{action=Index}/{id?}");

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();