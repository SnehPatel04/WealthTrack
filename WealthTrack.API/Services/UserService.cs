using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;
using WealthTrack.API.Models;

namespace WealthTrack.API.Services
{
    public class UserService
    {
        private readonly WealthTrackDbContext _context;

        public UserService(WealthTrackDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id              = u.Id,
                    FirstName       = u.FirstName,
                    LastName        = u.LastName,
                    Email           = u.Email,
                    Role            = u.Role,
                    IsActive        = u.IsActive,
                    CreatedAt       = u.CreatedAt,
                    AdvisorProfileId = u.Advisor != null ? u.Advisor.Id : (int?)null,
                    ClientProfileId  = u.Client  != null ? u.Client.Id  : (int?)null,
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id        = u.Id,
                    FirstName = u.FirstName,
                    LastName  = u.LastName,
                    Email     = u.Email,
                    Role      = u.Role,
                    IsActive  = u.IsActive,
                    CreatedAt = u.CreatedAt,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                FirstName    = dto.FirstName,
                LastName     = dto.LastName,
                Email        = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role         = dto.Role,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (dto.Role == "Advisor")
            {
                _context.Advisors.Add(new Advisor { UserId = user.Id });
                await _context.SaveChangesAsync();
            }

            if (dto.Role == "Customer")
            {
                _context.Clients.Add(new Client { UserId = user.Id });
                await _context.SaveChangesAsync();
            }

            return new UserDto
            {
                Id        = user.Id,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Email     = user.Email,
                Role      = user.Role,
                IsActive  = user.IsActive,
                CreatedAt = user.CreatedAt,
            };
        }

        public async Task<bool> SetUserActiveStatusAsync(int id, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return false;
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignAdvisorToClientAsync(AssignAdvisorDto dto)
        {
            var advisorExists = await _context.Advisors.AnyAsync(a => a.Id == dto.AdvisorId);
            var clientExists  = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);

            if (!advisorExists || !clientExists)
            {
                return false;
            }

            var alreadyAssigned = await _context.AdvisorClients
                .AnyAsync(ac => ac.AdvisorId == dto.AdvisorId && ac.ClientId == dto.ClientId);

            if (alreadyAssigned)
            {
                return true;
            }

            _context.AdvisorClients.Add(new AdvisorClient
            {
                AdvisorId  = dto.AdvisorId,
                ClientId   = dto.ClientId,
                AssignedAt = DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
