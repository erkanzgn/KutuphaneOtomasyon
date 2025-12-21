using Kutuphane.Application.Dtos.CopyDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Services;

public interface ICopyService
{
    Task<IEnumerable<ResultCopyDto>> GetCopiesByBookIdAsync(int bookId);
    Task<ResultCopyDto?> GetCopyByIdAsync(int id);
    Task<CopyWithDetailsDto?> GetCopyWithDetailsAsync(int id);
    Task<IEnumerable<ResultCopyDto>> GetAvailableCopiesAsync(int bookId);
    Task<ResultCopyDto> CreateCopyAsync(CreateCopyDto dto);
    Task<ResultCopyDto> UpdateCopyAsync(int id, UpdateCopyDto dto);
    Task DeleteCopyAsync(int id);
    Task AddCopiesAsync(int bookId, int count, string shelfLocation);
}
