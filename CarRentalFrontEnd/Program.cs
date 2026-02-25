using CarRentalFrontEnd;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Core services
builder.Services.AddHttpContextAccessor();

// Configure API Clients
Action<IServiceProvider, HttpClient> configureClient = (sp, client) =>
{
    var baseUrl = sp.GetRequiredService<IConfiguration>()["ApiSettings:BaseUrl"];
    if (!string.IsNullOrEmpty(baseUrl)) client.BaseAddress = new Uri(baseUrl);
};

builder.Services.AddHttpClient("CarRentalAPI", configureClient);
builder.Services.AddHttpClient<AuthService>(configureClient);

// Authentication: Cookie-based sign-in
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Auth endpoints
        options.LoginPath = "/Login_Register/Login";
        options.LogoutPath = "/Login_Register/Logout";

        options.AccessDeniedPath = "/Home/Unauthorized";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.Redirect("/Home/Unauthorized");
                return Task.CompletedTask;
            }
        };

        // Cookie lifetime
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Authorization: require authenticated user by default
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Session (stores JWT for API calls)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session expiry
});

var app = builder.Build();

// Diagnostic log for production verification
Console.WriteLine($"[DIAGNOSTIC] ApiSettings:BaseUrl is: {app.Configuration["ApiSettings:BaseUrl"]}");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
