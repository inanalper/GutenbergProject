using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using GutenbergPresentation.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using GutenbergProject.Models;

namespace GutenbergPresentation.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }
       
       
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(UserSignInModel model)
        {
            try
            {
                var userCredentials = new { Email = model.email, UserName = model.userName, Password = model.password };
                var json = JsonSerializer.Serialize(userCredentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync("https://localhost:7219/Authentication/SignIn", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewData["Message"] = "Registration successful.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Registration failed. Please try again.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewData["ErrorMessage"] = "An error occurred. Please try again.";
                return View();
            }
        }



        [HttpPost]
        public IActionResult Login(UserLoginModel model)
        {
            try
            {
                var userCredentials = new { UserName = model.userName, Password = model.password };
                var json = JsonSerializer.Serialize(userCredentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync("https://localhost:7219/Authentication/Authenticate", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var tokenString = response.Content.ReadAsStringAsync().Result;

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, model.userName));

                    var identity = new ClaimsIdentity(claims, "custom");
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(principal).Wait();

                    ViewData["Message"] = "Login successful.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Login failed. Please try again.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewData["ErrorMessage"] = "An error occurred. Please try again.";
                return View();
            }
        }











    }
}
