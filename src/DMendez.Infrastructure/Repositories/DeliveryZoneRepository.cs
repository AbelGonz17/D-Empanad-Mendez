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
    public class DeliveryZoneRepository : GenericRepository<DeliveryZone>, IDeliveryZoneRepository
    {
        public DeliveryZoneRepository(DMendezDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<DeliveryZone>> GetActiveZonesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(z => z.IsActive)
                .ToListAsync(cancellationToken);
        }
    }
}
