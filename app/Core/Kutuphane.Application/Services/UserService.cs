using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Entities;
using Kutuphane.Domain.Enums;
using BCrypt.Net;

namespace Kutuphane.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemberRepository _memberRepository; 

        public UserService(IUserRepository userRepository, IMemberRepository memberRepository)
        {
            _userRepository = userRepository;
            _memberRepository = memberRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = $"{u.FirstName} {u.LastName}",
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                MemberId = u.MemberId,
                LastLoginDate = u.LastLoginDate
            });
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User", id);

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                MemberId = user.MemberId,
                LastLoginDate = user.LastLoginDate
            };
        }

        // UserService.cs

        public async Task CreateUserAsync(CreateUserDto dto)
        {
          
            if (await _userRepository.IsUsernameExistsAsync(dto.Username))
                throw new DuplicateException("User", "Username", dto.Username);

            if (await _userRepository.IsEmailExistsAsync(dto.Email))
                throw new DuplicateException("User", "Email", dto.Email);

           
            var memberNumber = await GenerateUniqueMemberNumberAsync();

            var newMember = new Member
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                MemberNumber = memberNumber, 
                RegistrationDate = DateTime.Now,
                Status = MemberStatus.Aktif,
                Notes = "Sistem tarafından otomatik oluşturulan personel kaydı."
            };

            await _memberRepository.AddAsync(newMember);

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role,
                IsActive = true,
                CreatedDate = DateTime.Now,
                MemberId = newMember.Id
            };

            await _userRepository.AddAsync(newUser);
        }

       
        private async Task<string> GenerateUniqueMemberNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _memberRepository.CountAsync();
            var nextIndex = count + 1;

            while (true)
            {
                var candidateNumber = $"UYE-{year}-{nextIndex:D4}";

                // Veritabanında bu numara var mı diye kontrol et (Repository'nizde FirstOrDefaultAsync generic ise çalışır)
                var existingMember = await _memberRepository.FirstOrDefaultAsync(m => m.MemberNumber == candidateNumber);

                if (existingMember == null)
                {
                    // Eğer null ise bu numara boştadır, kullanılabilir.
                    return candidateNumber;
                }

                // Eğer doluysa sayacı 1 artırıp tekrar dene
                nextIndex++;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
        
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User", id);

            if (user.MemberId.HasValue)
            {
               
                var hasActiveLoans = await _memberRepository.HasActiveLoansAsync(user.MemberId.Value);
                if (hasActiveLoans)
                {
                    throw new BusinessException("Bu personelin üzerinde iade edilmemiş kitaplar var. Önce kitapları teslim alınız, sonra siliniz.");
                }

              
                await _memberRepository.DeleteAsync(user.MemberId.Value);
            }

            await _userRepository.DeleteAsync(id);
        }
        public async Task UpdateUserStatusAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User", id);

            user.IsActive = !user.IsActive;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserRoleAsync(int id, UserRole newRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User", id);

            user.Role = newRole;

            await _userRepository.UpdateAsync(user);
        }
        public async Task ResetUserPasswordAsync(int id, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User", id);

            // Hash the new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user);
        }
    }
}