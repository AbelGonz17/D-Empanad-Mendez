using System;

namespace DMendez.Domain.Entities
{
    public class UserAddress
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string AddressLine { get; private set; }
        public string Reference { get; private set; }
        public Guid? DeliveryZoneId { get; private set; }

        private UserAddress() {} // EF Core

        public UserAddress(string userId, string addressLine, string reference, Guid? deliveryZoneId = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(addressLine))
                throw new ArgumentException("La dirección es obligatoria.", nameof(addressLine));

            Id = Guid.NewGuid();
            UserId = userId;
            AddressLine = addressLine.Trim();
            Reference = reference?.Trim() ?? string.Empty;
            DeliveryZoneId = deliveryZoneId;
        }

        public void UpdateAddress(string addressLine, string reference, Guid? deliveryZoneId)
        {
            if (string.IsNullOrWhiteSpace(addressLine))
                throw new ArgumentException("La dirección es obligatoria.", nameof(addressLine));

            AddressLine = addressLine.Trim();
            Reference = reference?.Trim() ?? string.Empty;
            DeliveryZoneId = deliveryZoneId;
        }
    }
}
