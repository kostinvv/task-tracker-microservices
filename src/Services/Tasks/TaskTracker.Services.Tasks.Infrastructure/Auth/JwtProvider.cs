using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;
using TaskTracker.Services.Tasks.ApplicationCore.Models;
using TaskTracker.Services.Tasks.ApplicationCore.Options;

namespace TaskTracker.Services.Tasks.Infrastructure.Auth;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    
    public string GenerateJwtToken(ApplicationUser applicationUser)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
            new (ClaimTypes.Email, applicationUser.Email!),
        };
        var jwtSecurityToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiresHours),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                algorithm: SecurityAlgorithms.HmacSha256
                )
            );
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return jwtToken;
    }
}