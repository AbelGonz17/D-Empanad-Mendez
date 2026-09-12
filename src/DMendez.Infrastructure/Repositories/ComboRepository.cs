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
    public class ComboRepository : GenericRepository<Combo>, IComboRepository
    {
        public ComboRepository(DMendezDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Combo>> GetActiveCombosWithItemsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Items)
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Combo?> GetComboWithItemsAsync(Guid comboId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == comboId, cancellationToken);
        }
    }
}
