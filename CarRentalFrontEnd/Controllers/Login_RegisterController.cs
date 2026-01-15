using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalFrontEnd.Controllers
{
    public class Login_RegisterController : Controller
    {
        private readonly AuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Login_RegisterController(AuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            try
            {
                var isAuthenticated = await _authService.AuthenticateUserAsync(Username, Password);

                if (!isAuthenticated)
                {
                    ViewBag.Error = "Invalid username or password. Please try again.";
                    return View();
                }

                // Set a persistent cookie that lasts for 30 days
                Response.Cookies.Append("AuthToken", "authenticated", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(30)
                });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
                ViewBag.Error = $"An error occurred during login: {ex.Message}";
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Models.RegisterModel model, [FromServices] IHttpClientFactory clientFactory)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Reuse the same backend logic as UserController.AddEdit by calling POST api/User
                var client = clientFactory.CreateClient("CarRentalAPI");

                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent("0"), "UserId");
                formData.Add(new StringContent(model.Name ?? string.Empty), "Name");
                formData.Add(new StringContent(model.Email ?? string.Empty), "Email");
                formData.Add(new StringContent(model.Password ?? string.Empty), "Password");
                formData.Add(new StringContent(model.Phone ?? string.Empty), "Phone");
                formData.Add(new StringContent("Customer"), "Role");

                var response = await client.PostAsync("user", formData);

                if (!response.IsSuccessStatusCode)
                {
                    var detail = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Registration failed. Please try again or contact support.";
                    ViewBag.ErrorDetails = detail;
                    return View(model);
                }

                // Success: redirect to Login
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("Login", "Login_Register");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred during registration: {ex.Message}";
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            HttpContext.Session.Clear();

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Login", "Login_Register");
        }
    }
}
