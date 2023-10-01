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
            var jwtSettings = Program.App.Configuration.GetSection("JWT");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = "JobBoard",
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
