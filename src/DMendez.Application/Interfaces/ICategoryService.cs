using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.Categories;

namespace DMendez.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<IReadOnlyList<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<CategoryDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
        Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);
        Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
