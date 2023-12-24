
using GutenbergProject.Entities;
using GutenbergProject.JwtConfig;
using GutenbergProject.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly JwtTokenGenerator _JwtTokenGenerator;
        public AuthenticationController(MyContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _JwtTokenGenerator = jwtTokenGenerator; 

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

        [Authorize]
        [HttpPost("SignIn", Name = "SignIn")]
        public IActionResult SignIn(UserSignInModel model)
        {
            try
            {
             
                var passwordHash = ComputeMD5Hash(model.password);
                var existingUser = _context.Users.FirstOrDefault(u => u.email == model.email || u.userName == model.userName);
                if (existingUser == null)
                { 
                    var newUser = new User(model.email,model.userName, passwordHash);


                  _context.Users.Add(newUser);
                  _context.SaveChanges();

                  return Ok();

                }
                else
                {
                    return BadRequest();
                }
               
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error"); 
            }
        }

        [HttpPost("Authenticate", Name = "Authenticate")]
        public IActionResult Login([FromBody]UserLoginModel model)
        {
            try
            {
                User user = _context.Users.FirstOrDefault(u => u.userName == model.userName);
                if (user == null)
                {
                    return StatusCode(400, "No user found with this username");
                }
                else
                {
                    if (user.passwordHash == ComputeMD5Hash(model.password))
                    {
                        string tokenString = _JwtTokenGenerator.GenerateToken(user);
                        return Ok(tokenString);
                    }
                    else
                    {
                        return StatusCode(401, "Password is wrong.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred.");
            }
        }



    }
}
