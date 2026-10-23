using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;
using WealthTrack.API.Models;

namespace WealthTrack.API.Services
{
    public class AuthService
    {
        private readonly WealthTrackDbContext _context;
        private readonly TokenService _tokenService;

        public AuthService(WealthTrackDbContext context, TokenService tokenService)
        {
            _context      = context;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user == null)
            {
                return null;
            }

            var passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!passwordIsValid)
            {
                return null;
            }

            var token = _tokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token     = token,
                Email     = user.Email,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Role      = user.Role,
                UserId    = user.Id,
            };
        }
    }
}
