using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Middlware;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMPLOYEE.Application.Abstractions
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly IPermissionService _permissionService;
        private readonly SymmetricSecurityKey _key;
       
        public JwtService(IConfiguration config, IPermissionService permissionService)
        {
            _config = config;
            _permissionService = permissionService;
            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );
           
        }

        public async Task<string> GenerateTokenAsync(string userId, string email, IList<string> roles)
        {
        

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("uid", userId)
        };
            
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            
            var permissions = await _permissionService.GetPermissionsAsync(userId);
 

            foreach (var permission in permissions)
                claims.Add(new Claim("permission", permission));

        
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );
            
          
            return new JwtSecurityTokenHandler().WriteToken(token);
           
        }
    }

}
