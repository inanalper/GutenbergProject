﻿using System.IdentityModel.Tokens.Jwt;

namespace GutenbergProject.Service
{
    public interface IJWTDecoder
    {
        string GetUserIdFromToken(string token);
    }

    public class JWTDecoder : IJWTDecoder
    {
        public string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null) return null;

                var nameIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;
                return nameIdClaim;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as appropriate
                throw;
            }
        }
    }
}