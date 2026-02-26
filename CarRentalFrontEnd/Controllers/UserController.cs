using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateAuthorizedClient()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("JWTToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "User Management";
            ViewData["PageTitle"] = "User List";
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.GetAsync("user");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                
                List<UserModel> list = new List<UserModel>();
                try
                {
                    var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                    if (obj["$values"] != null) 
                        list = obj["$values"].ToObject<List<UserModel>>() ?? new List<UserModel>();
                    else if (obj["data"] != null)
                        list = obj["data"].ToObject<List<UserModel>>() ?? new List<UserModel>();
                }
                catch 
                {
                    // Fallback to simple array
                    list = JsonConvert.DeserializeObject<List<UserModel>>(json) ?? new List<UserModel>();
                }

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load users.";
                return View(new List<UserModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "User Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add User" : "Edit User";
            if (id == null || id == 0)
                return View(new UserModel());

            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"user/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load user.";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserModel>(data);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(UserModel user)
        {
            var client = CreateAuthorizedClient();

            // Create multipart form data for file upload
            using var formData = new MultipartFormDataContent();

            // Add user data as form fields
            formData.Add(new StringContent(user.UserId.ToString()), "UserId");
            formData.Add(new StringContent(user.Name ?? ""), "Name");
            formData.Add(new StringContent(user.Email ?? ""), "Email");
            formData.Add(new StringContent(user.Password ?? ""), "Password");
            formData.Add(new StringContent(user.Phone ?? ""), "Phone");
            formData.Add(new StringContent(user.Role ?? "Customer"), "Role");

            // Add profile image if provided
            if (user.ProfileImage != null && user.ProfileImage.Length > 0)
            {
                var imageContent = new StreamContent(user.ProfileImage.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(user.ProfileImage.ContentType);
                formData.Add(imageContent, "profileImage", user.ProfileImage.FileName);
            }

            HttpResponseMessage response;
            if (user.UserId == 0)
                response = await client.PostAsync("user", formData);
            else
                response = await client.PutAsync($"user/{user.UserId}", formData);

            if (response.IsSuccessStatusCode)
            {
                // If image was uploaded, copy it to frontend project
                if (user.ProfileImage != null && user.ProfileImage.Length > 0)
                {
                    try
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var userResponse = JsonConvert.DeserializeObject<UserModel>(responseData);

                        if (!string.IsNullOrEmpty(userResponse?.ProfileImagePath))
                        {
                            // Copy the uploaded image from API to frontend
                            var apiImagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "CarRental", "wwwroot", userResponse.ProfileImagePath);
                            var frontendImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userResponse.ProfileImagePath);

                            if (System.IO.File.Exists(apiImagePath))
                            {
                                var frontendDir = Path.GetDirectoryName(frontendImagePath);
                                if (!Directory.Exists(frontendDir))
                                    Directory.CreateDirectory(frontendDir);

                                System.IO.File.Copy(apiImagePath, frontendImagePath, true);
                                Console.WriteLine($"Copied image from {apiImagePath} to {frontendImagePath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error copying image: {ex.Message}");
                    }
                }

                TempData["SuccessMessage"] = user.UserId == 0
                    ? "User created successfully."
                    : "User updated successfully.";
                return RedirectToAction("Index");
            }

            var responseText = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = "Failed to save user.";
            ViewBag.ErrorDetails = responseText;
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"user/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "User deleted.";
            else
                TempData["ErrorMessage"] = "Failed to delete user.";

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfileImage(int id)
        {
            try
            {
                var client = CreateAuthorizedClient();
                var response = await client.DeleteAsync($"user/profile-picture/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Profile image deleted successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to delete profile image." });
                }
            }
            catch
            {
                return BadRequest(new { message = "Failed to delete profile image." });
            }
        }
    }
}
