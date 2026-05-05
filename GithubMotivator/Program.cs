
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNet.Security.OAuth.GitHub;
using GithubMotivator.Data;
using GithubMotivator.Models;
using GithubMotivator.Services;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGithubService, GithubService>();
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
builder.Services.AddHostedService<CommitPollingWorker>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
})

.AddCookie(options =>
{
    options.Cookie.Name = "GitMotivator.Auth";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GithubMotivator",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "GithubMotivator",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForLocalDevelopment"))
    };
})

.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["GitHub:ClientId"] ?? throw new InvalidOperationException("GitHub ClientId is missing.");
    options.ClientSecret = builder.Configuration["GitHub:ClientSecret"] ?? throw new InvalidOperationException("GitHub ClientSecret is missing.");
    options.CallbackPath = "/signin-github";
    options.Scope.Add("user:email");
    options.SaveTokens = true;

    options.CorrelationCookie.Name = "GitMotivator.Correlation";
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    options.Events.OnCreatingTicket = async context =>
    {
        var username = context.Identity?.FindFirst(ClaimTypes.Name)?.Value;
        var email = context.Identity?.FindFirst(ClaimTypes.Email)?.Value;
        var accessToken = context.AccessToken;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(accessToken)) return;

        using var scope = context.HttpContext.RequestServices.CreateScope();
        var githubService = scope.ServiceProvider.GetRequiredService<IGithubService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        int commitCount = await githubService.GetUserCommitCountAsync(username, accessToken);
        var client = new GitHubClient(new ProductHeaderValue("GithubMotivator"))
        {
            Credentials = new Credentials(accessToken)
        };
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            user = new GithubMotivator.Models.User
            {
                Username = username,
                Email = email ?? "",
                Name = context.Identity?.FindFirst("urn:github:name")?.Value ?? username,
                GitHubToken = accessToken,
                Commits = commitCount,
                Type = GithubMotivator.Models.User.UserType.Contributor
            };
            dbContext.Users.Add(user);
        }
        else
        {
            user.GitHubToken = accessToken;
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
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
    options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Ensure Database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseCookiePolicy();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
        name: "api",
        pattern: "api/{controller}/{action=Index}/{id?}");

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();