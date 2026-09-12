using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;

namespace DMendez.Domain.Interfaces
{
    public interface IDeliveryZoneRepository : IGenericRepository<DeliveryZone>
    {
        Task<IReadOnlyList<DeliveryZone>> GetActiveZonesAsync(CancellationToken cancellationToken = default);
    }
}
