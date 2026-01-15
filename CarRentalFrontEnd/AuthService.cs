using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace CarRentalFrontEnd
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Login: verify credentials with API, create auth cookie, save JWT & role in session
        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                // Build form data for login
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Name", username),
                    new KeyValuePair<string, string>("Password", password)
                });

                // POST to backend login endpoint
                var response = await _httpClient.PostAsync("http://localhost:5075/api/User/login", formData);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Login failed. Status: {response.StatusCode}, Response: {responseContent}");
                    return false;
                }

                // Extract JWT token from response JSON
                var token = ExtractToken(responseContent);
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine($"Token not found in response: {responseContent}");
                    return false;
                }

                // Decode JWT to read claims
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // Build claims list and capture role
                var (claims, userRole) = BuildClaims(jwt);
                // Also add the raw token if needed later
                claims.Add(new Claim("JWT", token));

                // Create cookie principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in with cookie (persistent, 30 mins)
                await _httpContextAccessor.HttpContext!.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

                // Save JWT & role in session for authorized API calls and UI
                if (_httpContextAccessor.HttpContext.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("JWTToken", token);
                    _httpContextAccessor.HttpContext.Session.SetString("UserRole", userRole);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Session is not available");
                }

                System.Diagnostics.Debug.WriteLine("Login successful");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Authentication error: {ex}");
                throw; // This will be caught by the controller
            }
        }

        // Register: send new user payload to backend
        public async Task<bool> RegisterUserAsync(string name, string email, string phone, string password)
        {
            try
            {
                // Prepare request body
                var userData = new
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Password = password,
                    Role = "Customer" // Default role for new registrations
                };

                var json = System.Text.Json.JsonSerializer.Serialize(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // POST to backend register endpoint
                var response = await _httpClient.PostAsync("http://localhost:5075/api/User", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Registration failed. Status: {response.StatusCode}, Response: {responseContent}");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine("Registration successful");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex}");
                throw; // This will be caught by the controller
            }
        }

        // Logout: clear cookie auth, session and browser cookies
        public async Task LogoutAsync()
        {
            // Clear auth cookie
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session
            _httpContextAccessor.HttpContext.Session.Clear();

            // Clear all cookies
            foreach (var cookie in _httpContextAccessor.HttpContext.Request.Cookies.Keys)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie);
            }
        }

        // Helpers
        private static string? ExtractToken(string responseContent)
        {
            try
            {
                var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                return jsonDoc.RootElement.TryGetProperty("token", out var tokenElement)
                    ? tokenElement.GetString()
                    : null;
            }
            catch
            {
                return null;
            }
        }

        // Parse claims from JWT and return along with detected role
        private static (List<Claim> claims, string userRole) BuildClaims(JwtSecurityToken jwt)
        {
            var claims = new List<Claim>();
            string role = "User";
            foreach (var claim in jwt.Claims)
            {
                claims.Add(claim);
                if (claim.Type == ClaimTypes.Role)
                {
                    role = claim.Value;
                }
            }
            return (claims, role);
        }
    }
}