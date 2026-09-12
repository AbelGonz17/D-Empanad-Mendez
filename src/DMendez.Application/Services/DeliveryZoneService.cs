using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.DeliveryZones;
using DMendez.Application.Interfaces;
using DMendez.Application.Mappings;
using DMendez.Domain.Interfaces;

namespace DMendez.Application.Services
{
    public class DeliveryZoneService : IDeliveryZoneService
    {
        private readonly IDeliveryZoneRepository _zoneRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryZoneService(IDeliveryZoneRepository zoneRepository, IUnitOfWork unitOfWork)
        {
            _zoneRepository = zoneRepository ?? throw new ArgumentNullException(nameof(zoneRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<IReadOnlyList<DeliveryZoneDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var zones = await _zoneRepository.GetAllAsync(cancellationToken);
            return Result.Success(zones.ToDtoList());
        }

        public async Task<Result<IReadOnlyList<DeliveryZoneDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var zones = await _zoneRepository.GetActiveZonesAsync(cancellationToken);
            return Result.Success(zones.ToDtoList());
        }

        public async Task<Result<DeliveryZoneDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return Result.NotFound<DeliveryZoneDto>($"No se encontró la zona de entrega con ID '{id}'.");

            return Result.Success(zone.ToDto());
        }

        public async Task<Result<DeliveryZoneDto>> CreateAsync(CreateDeliveryZoneDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<DeliveryZoneDto>("El nombre de la zona es requerido.");

            if (dto.DeliveryFee < 0)
                return Result.Failure<DeliveryZoneDto>("La tarifa de entrega no puede ser negativa.");

            var zone = dto.ToEntity();
            await _zoneRepository.AddAsync(zone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(zone.ToDto());
        }

        public async Task<Result<DeliveryZoneDto>> UpdateAsync(Guid id, UpdateDeliveryZoneDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<DeliveryZoneDto>("El nombre de la zona es requerido.");

            if (dto.DeliveryFee < 0)
                return Result.Failure<DeliveryZoneDto>("La tarifa de entrega no puede ser negativa.");

            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return Result.NotFound<DeliveryZoneDto>($"No se encontró la zona de entrega con ID '{id}'.");

            zone.Rename(dto.Name);
            zone.ChangeDeliveryFee(dto.DeliveryFee);

            _zoneRepository.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(zone.ToDto());
        }

        public async Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return Result.NotFound($"No se encontró la zona de entrega con ID '{id}'.");

            zone.Activate();
            _zoneRepository.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return Result.NotFound($"No se encontró la zona de entrega con ID '{id}'.");

            zone.Deactivate();
            _zoneRepository.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return Result.NotFound($"No se encontró la zona de entrega con ID '{id}'.");

            _zoneRepository.Delete(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
