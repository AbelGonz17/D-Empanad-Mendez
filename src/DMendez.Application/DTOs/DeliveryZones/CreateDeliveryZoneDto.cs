namespace DMendez.Application.DTOs.DeliveryZones
{
    public class CreateDeliveryZoneDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
    }
}
