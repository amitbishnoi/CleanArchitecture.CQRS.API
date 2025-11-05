using Application.Features.Authentication.Dtos;
using Application.Features.Authentication.Interfaces;
using Application.Interfaces;
using Application.Interfaces.SecurityInterface;
using Microsoft.Extensions.Options;

namespace Infrastructure.Security
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse?> AuthenticateAsync(LoginRequest request)
        {
            var user = await _unitOfWork.Users
                .GetAsync(u => u.Email == request.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = _tokenService.GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
            };
        }
    }
}