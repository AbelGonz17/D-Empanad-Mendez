namespace DMendez.Domain.Entities
{
    public class DeliveryZone
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal DeliveryFee { get; private set; }
        public bool IsActive { get; private set; }

        private DeliveryZone() {} // EF Core

        public DeliveryZone(string name, decimal deliveryFee)
        {
            Id = Guid.NewGuid();
            Name = ValidateName(name);

            ChangeDeliveryFee(deliveryFee);

            IsActive = true;
        }

        public void Rename(string name)
        {
            Name = ValidateName(name);
        }

        public void ChangeDeliveryFee(decimal deliveryFee)
        {
            if (deliveryFee < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deliveryFee),
                    "La tarifa de entrega no puede ser negativa.");
            }

            DeliveryFee = deliveryFee;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre de la zona es obligatorio.",
                    nameof(name));
            }

            return name.Trim();
        }
    }
}