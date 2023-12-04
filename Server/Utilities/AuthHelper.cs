using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace job_board.Utilities
{
    public static class AuthHelper
    {
        public static (string salt, string hashedPassword) HashPassword(string password)
        {
            byte[] passwordPlain = Encoding.UTF8.GetBytes(password);
            byte[] passwordSalt = RandomNumberGenerator.GetBytes(32);

            string saltString, passwordHashString;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordPlain, passwordSalt, 10000, HashAlgorithmName.SHA512))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                saltString = Convert.ToBase64String(passwordSalt);
                passwordHashString = Convert.ToBase64String(hash);
            }
            return (saltString, passwordHashString);
        }
        
        public static bool DoesPasswordMatch(string loginPassword, string storedSalt, string storedPasswordHash)
        {
            byte[] passwordPlain = Encoding.UTF8.GetBytes(loginPassword);
            byte[] passwordSalt = Convert.FromBase64String(storedSalt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordPlain, passwordSalt, 10000, HashAlgorithmName.SHA512))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                string passwordHashString = Convert.ToBase64String(hash);
                return passwordHashString == storedPasswordHash;
            }
        }

        public static string GenerateJwtToken(int userId, string role)
        {
            var jwtKey = Program.Application.Configuration.GetValue<string>("Jwt_Key");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Program.Application.Configuration.GetValue<string>("Jwt_Issuer"),
                Audience = "JobBoard",
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || int.TryParse(userIdClaim.Value, out int userId) == false)
            {
                return 0;
            }
            return userId;
        }
    }
}
