using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;

namespace DMendez.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<Product?> GetWithInventoryAndDiscountsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
