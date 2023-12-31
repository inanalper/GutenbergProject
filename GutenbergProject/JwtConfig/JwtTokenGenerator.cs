using GutenbergProject.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GutenbergProject.JwtConfig
{
    public class JwtTokenGenerator
    {


        private readonly IConfiguration _config;
        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }


        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,user.userName),
                            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                        };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTKey:Secret"]));
           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["JWTKey:ValidIssuer"],
                Audience = _config["JWTKey:ValidAudience"],
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

