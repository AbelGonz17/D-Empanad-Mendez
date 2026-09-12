using System;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;
using DMendez.Domain.Interfaces;
using DMendez.Infrastructure.Contex;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Infrastructure.Repositories
{
    public class ProductInventoryRepository : GenericRepository<ProductInventory>, IProductInventoryRepository
    {
        public ProductInventoryRepository(DMendezDbContext context) : base(context)
        {
        }

        public async Task<ProductInventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pi => pi.ProductId == productId, cancellationToken);
        }
    }
}
