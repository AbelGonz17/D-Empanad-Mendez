using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Products;

namespace DMendez.Application.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductDto>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken = default);
        Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
