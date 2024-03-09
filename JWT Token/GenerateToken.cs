using Dental_Manager.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dental_Manager.JWT_Token
{
    public class GenerateToken
    {
        private string GenerateRandomKeys(int length)
        {
            var random = new RNGCryptoServiceProvider();
            byte[] keyBytes = new byte[length / 8];
            random.GetBytes(keyBytes);

            return Convert.ToBase64String(keyBytes);
        }

        public string CreateToken(Employee employee)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.EmployeeName ?? string.Empty),
                new Claim(ClaimTypes.Email, employee.EmployeeEmail ?? string.Empty),
                new Claim(ClaimTypes.Role, employee.RoleId.ToString()),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GenerateRandomKeys(512)));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
