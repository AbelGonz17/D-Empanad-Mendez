using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.DeliveryZones;

namespace DMendez.Application.Interfaces
{
    public interface IDeliveryZoneService
    {
        Task<IReadOnlyList<DeliveryZoneDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DeliveryZoneDto>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<DeliveryZoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DeliveryZoneDto> CreateAsync(CreateDeliveryZoneDto dto, CancellationToken cancellationToken = default);
        Task<DeliveryZoneDto?> UpdateAsync(Guid id, UpdateDeliveryZoneDto dto, CancellationToken cancellationToken = default);
        Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
