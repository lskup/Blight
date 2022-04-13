using Blight.Entieties;
using Blight.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Authentication
{
    public class SchemesGenerator:ISchemeGenerator
    {
        private readonly AuthenticationSettings _authenticationSettings;

        public SchemesGenerator(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        public string GenerateJWT(User existingUser)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{existingUser.FirstName} {existingUser.LastName}"),
                new Claim(ClaimTypes.Role, $"{existingUser.Role.Name}"),
                new Claim("DateOfBirth", existingUser.DateOfBirth.ToShortDateString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }

    }
}
