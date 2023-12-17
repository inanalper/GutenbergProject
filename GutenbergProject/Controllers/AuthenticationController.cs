using GutenbergProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GutenbergProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly MyContext _context;

        public AuthenticationController(MyContext context)
        {
            _context = context;
        }

        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        [HttpPost("SignIn", Name = "SignIn")]
        public IActionResult SignIn([FromQuery] string userName, [FromQuery] string password)
        {
            User user = new User(userName, ComputeMD5Hash(password));
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok(); // 200 OK
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return StatusCode(500, "Internal server error"); // 500 Internal Server Error
            }
        }

        [HttpPost("Authenticate", Name = "Authenticate")]
        public IActionResult login([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                User user = _context.Users.FirstOrDefault(u => u.userName == username);
                if(user == null)
                {
                    return StatusCode(400, "No user found with this username");
                } else
                {
                    if(user.passwordHash == ComputeMD5Hash(password))
                    {
                        return Ok();
                    } else
                    {
                        return StatusCode(401, "Password is wrong.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured.");
            }

        }
    }
}
