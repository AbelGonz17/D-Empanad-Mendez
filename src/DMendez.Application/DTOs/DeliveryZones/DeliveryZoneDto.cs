using System;

namespace DMendez.Application.DTOs.DeliveryZones
{
    public class DeliveryZoneDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public bool IsActive { get; set; }
    }
}
