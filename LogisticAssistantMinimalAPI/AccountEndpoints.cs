using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogisticAssistantMinimalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LogisticAssistantMinimalAPI
{
    public static class AccountEndpoints
    {
        public static void MapAccountEndpoints(WebApplication app, string jwtKey)
        {
            app.MapPost("/api/accounts/register", async (AppDbContext db, string name, string password) =>
            {
                if (await db.Users.AnyAsync(u => u.Name == name))
                    return Results.BadRequest("User already exists");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new User
                {
                    Name = name,
                    PasswordHashed = hashedPassword
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Ok("User created");
            });

            app.MapPost("/api/accounts/login", async (AppDbContext db, string name, string password) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Name == name);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHashed))
                    return Results.Unauthorized();

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("Id", user.Id.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { token = tokenString });
            });

        }
    }
}