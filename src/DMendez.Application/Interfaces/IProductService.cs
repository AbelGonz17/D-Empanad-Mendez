using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.Products;

namespace DMendez.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<IReadOnlyList<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<ProductDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<ProductDto>>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
        Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
        Task<Result<ProductDto>> UpdateStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken = default);
        Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
