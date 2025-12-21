using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task CreateUserAsync(CreateUserDto dto);
        Task DeleteUserAsync(int id);
        Task UpdateUserStatusAsync(int id); 
        Task UpdateUserRoleAsync(int id, UserRole newRole);
        Task ResetUserPasswordAsync(int id, string newPassword);
    }
}
