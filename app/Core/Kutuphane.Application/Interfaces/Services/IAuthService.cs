using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Dtos.MemberDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDto?> LoginAsync(LoginDto dto);
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> UpdateUserRoleAsync(int userId, string role);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task DeactivateUserAsync(int userId);
    Task<IEnumerable<Claim>> GetUserClaimsAsync(int userId);
}
