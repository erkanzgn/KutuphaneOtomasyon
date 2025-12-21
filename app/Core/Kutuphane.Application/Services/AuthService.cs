using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Dtos.MemberDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Entities;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Services;

public class AuthService:IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMemberRepository _memberRepository;
   
    public AuthService(IUserRepository userRepository, IMemberRepository memberRepository)
    {
        _userRepository = userRepository;
        _memberRepository = memberRepository;
    }

    public async Task<AuthResultDto?> LoginAsync(LoginDto dto)
    {
        // 1. Kullanıcıyı bul
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null)
        {
            return null; // Kullanıcı bulunamadı
        }

        // 2. Kullanıcı aktif mi?
        if (!user.IsActive)
        {
            throw new UnauthorizedException("Hesabınız devre dışı bırakıldı. Lütfen yöneticiyle iletişime geçin.");
        }

        // 3. Şifre doğrulama (BCrypt)
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null; // Şifre yanlış
        }

        // 4. LastLoginDate güncelle
        user.LastLoginDate = DateTime.Now;
        await _userRepository.UpdateAsync(user);

        // 5. Claims oluştur
        var claims = await GetUserClaimsAsync(user.Id);

        // 6. Sonuç döndür
        return new AuthResultDto
        {
            User = MapToUserDto(user),
            Claims = claims
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Username kontrolü
        if (await _userRepository.IsUsernameExistsAsync(dto.Username))
        {
            throw new DuplicateException("User", "Username", dto.Username);
        }

        // 2. Email kontrolü
        if (await _userRepository.IsEmailExistsAsync(dto.Email))
        {
            throw new DuplicateException("User", "Email", dto.Email);
        }

        // 3. Şifre hash'le
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // 4. User oluştur
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = Enum.Parse<UserRole>(dto.Role), // "Member" → UserRole.Member
            IsActive = true,
            MemberId = dto.MemberId // ✅ Controller'dan gelen MemberId
        };

        await _userRepository.AddAsync(user);

        return MapToUserDto(user);
    }

    private async Task<string> GenerateMemberNumberAsync()
    {
        var year = DateTime.Now.Year;
        var count = await _memberRepository.CountAsync();
        return $"UYE-{year}-{(count + 1):D4}";
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto> UpdateUserRoleAsync(int userId, string role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }

        if (!Enum.TryParse<UserRole>(role, out var userRole))
        {
            throw new BusinessException($"Invalid role: {role}");
        }

        user.Role = userRole;
        await _userRepository.UpdateAsync(user);

        return MapToUserDto(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException("User", userId);

        // A) Eski şifre doğru mu?
        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
        {
            throw new BusinessException("Mevcut şifrenizi yanlış girdiniz.");
        }

        // B) Yeni şifre, eski şifreyle aynı mı? (Opsiyonel Güvenlik Kuralı)
        if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
        {
            throw new BusinessException("Yeni şifreniz eskisiyle aynı olamaz.");
        }

        // C) Yeni şifreyi hashle ve kaydet
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeactivateUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }

        // Son admin kontrolü
        if (user.Role == UserRole.Admin)
        {
            var adminCount = await _userRepository.CountAsync(u => u.Role == UserRole.Admin && u.IsActive);
            if (adminCount <= 1)
            {
                throw new BusinessException("Son aktif yönetici kullanıcısını devre dışı bırakamazsınız");
            }
        }

        user.IsActive = false;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

        return claims;
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            MemberId = user.MemberId,
            LastLoginDate = user.LastLoginDate
        };
    }
}
