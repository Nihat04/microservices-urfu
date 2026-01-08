using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Contracts.Users;
using UserService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Repository;


namespace UserService.Services;

public class AuthService
{
    private readonly IUserRepository  _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(IUserRepository  userRepository)
    {
        _userRepository = userRepository;
        _passwordHasher =  new PasswordHasher<User>();
    }
    
    public async Task<RegisterResponce> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.FullName
        };
        user.HashedPassword = _passwordHasher.HashPassword(user, request.Password);
        await _userRepository.CreateAsync(user);
        var res = new RegisterResponce(
            Id: user.Id,
            Email: user.Email,
            Name: user.Name
        );
        return res; 
    }
    
    public async Task<AuthResponce?> LoginAsync(LoginRequest request)
    {
        var user =  await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return null;
        }
        var passVerifyRes = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, request.Password);
        if (passVerifyRes == PasswordVerificationResult.Success)
        {
            var token = GenerateJwtToken(user);
            return new AuthResponce
            {
                Token = token,
            };
        }
        else if (passVerifyRes == PasswordVerificationResult.Failed)
            return null;
        return null;
    }

    //TODO: Убрать в кофнигурацию 
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(4),
            claims: claims,
            signingCredentials:
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("secretKey1234567890secretKey1234567890")), SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}