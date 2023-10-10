using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TouristarModels.Models;
using TouristarBackend.Contracts;

namespace TouristarBackend.Services;

public class JwtService : IJwtService
{
    public string CreateToken(long userId, string? tokenKey)
    {
        if (tokenKey == null)
        {
            throw new InvalidOperationException("tokenKey cannot be null.");
        }

        List<Claim> claims = new() { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
