using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.DeliveryZones;

namespace DMendez.Application.Interfaces
{
    public interface IDeliveryZoneService
    {
        Task<Result<IReadOnlyList<DeliveryZoneDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<DeliveryZoneDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<Result<DeliveryZoneDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<DeliveryZoneDto>> CreateAsync(CreateDeliveryZoneDto dto, CancellationToken cancellationToken = default);
        Task<Result<DeliveryZoneDto>> UpdateAsync(Guid id, UpdateDeliveryZoneDto dto, CancellationToken cancellationToken = default);
        Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
