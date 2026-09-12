using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;
using DMendez.Domain.Interfaces;
using DMendez.Infrastructure.Contex;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DMendezDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetWithInventoryAndDiscountsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
