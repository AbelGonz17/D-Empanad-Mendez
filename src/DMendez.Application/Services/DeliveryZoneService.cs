using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.DeliveryZones;
using DMendez.Application.Interfaces;
using DMendez.Application.Mappings;
using DMendez.Domain.Entities;
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

        public async Task<IReadOnlyList<DeliveryZoneDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var zones = await _zoneRepository.GetAllAsync(cancellationToken);
            return zones.ToDtoList();
        }

        public async Task<IReadOnlyList<DeliveryZoneDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var zones = await _zoneRepository.GetActiveZonesAsync(cancellationToken);
            return zones.ToDtoList();
        }

        public async Task<DeliveryZoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            return zone?.ToDto();
        }

        public async Task<DeliveryZoneDto> CreateAsync(CreateDeliveryZoneDto dto, CancellationToken cancellationToken = default)
        {
            var zone = dto.ToEntity();
            await _zoneRepository.AddAsync(zone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return zone.ToDto();
        }

        public async Task<DeliveryZoneDto?> UpdateAsync(Guid id, UpdateDeliveryZoneDto dto, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return null;

            zone.Rename(dto.Name);
            zone.ChangeDeliveryFee(dto.DeliveryFee);

            _zoneRepository.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return zone.ToDto();
        }

        public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return false;

            zone.Activate();
            _zoneRepository.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return false;

            zone.Deactivate();
            _zoneRepository.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var zone = await _zoneRepository.GetByIdAsync(id, cancellationToken);
            if (zone == null)
                return false;

            _zoneRepository.Delete(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
