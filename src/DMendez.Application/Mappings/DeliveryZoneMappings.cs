using System.Collections.Generic;
using System.Linq;
using DMendez.Application.DTOs.DeliveryZones;
using DMendez.Domain.Entities;

namespace DMendez.Application.Mappings
{
    public static class DeliveryZoneMappings
    {
        public static DeliveryZoneDto ToDto(this DeliveryZone zone)
        {
            return new DeliveryZoneDto
            {
                Id = zone.Id,
                Name = zone.Name,
                DeliveryFee = zone.DeliveryFee,
                IsActive = zone.IsActive
            };
        }

        public static IReadOnlyList<DeliveryZoneDto> ToDtoList(this IEnumerable<DeliveryZone> zones)
        {
            return zones.Select(z => z.ToDto()).ToList();
        }

        public static DeliveryZone ToEntity(this CreateDeliveryZoneDto dto)
        {
            return new DeliveryZone(dto.Name, dto.DeliveryFee);
        }
    }
}
