using System;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;

namespace DMendez.Domain.Interfaces
{
    public interface IProductInventoryRepository : IGenericRepository<ProductInventory>
    {
        Task<ProductInventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
