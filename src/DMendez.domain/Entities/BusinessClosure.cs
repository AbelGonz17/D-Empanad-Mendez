namespace DMendez.Domain.Entities
{
    public class BusinessClosure
    {
        public Guid Id { get; private set; }
        public DateOnly Date { get; private set; }
        public string Reason { get; private set; }
        public bool IsActive { get; private set; }

        private BusinessClosure() {} // EF Core

        public BusinessClosure(DateOnly date, string reason)
        {
            Id = Guid.NewGuid();
            Date = date;
            Reason = ValidateReason(reason);
            IsActive = true;
        }

        public void ChangeReason(string reason)
        {
            Reason = ValidateReason(reason);
        }

        public void Cancel()
        {
            IsActive = false;
        }

        public bool AppliesTo(DateOnly date)
        {
            return IsActive && Date == date;
        }

        private static string ValidateReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException(
                    "El motivo del cierre es obligatorio.",
                    nameof(reason));
            }

            return reason.Trim();
        }
    }
}