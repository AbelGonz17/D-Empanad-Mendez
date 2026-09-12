namespace DMendez.Domain.Entities
{
    public class BusinessHours
    {
        public Guid Id { get; private set; }
        public DayOfWeek Day { get; private set; }
        public TimeOnly OpeningTime { get; private set; }
        public TimeOnly ClosingTime { get; private set; }
        public bool IsClosed { get; private set; }

        public TimeOnly OrderCutoffTime => ClosingTime.AddMinutes(-20);

        private BusinessHours() {} // EF Core

        public BusinessHours(
            DayOfWeek day,
            TimeOnly openingTime,
            TimeOnly closingTime)
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), day))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(day),
                    "El día de la semana no es válido.");
            }

            Id = Guid.NewGuid();
            Day = day;

            ChangeSchedule(openingTime, closingTime);
        }

        public void ChangeSchedule(
            TimeOnly openingTime,
            TimeOnly closingTime)
        {
            if (closingTime <= openingTime)
            {
                throw new ArgumentException(
                    "El cierre debe ser posterior a la apertura.");
            }

            var duration = closingTime - openingTime;

            if (duration <= TimeSpan.FromMinutes(20))
            {
                throw new ArgumentException(
                    "El horario debe durar más de 20 minutos.");
            }

            OpeningTime = openingTime;
            ClosingTime = closingTime;
        }

        public void Close()
        {
            IsClosed = true;
        }

        public void Open()
        {
            IsClosed = false;
        }

        public bool CanAcceptOrders(TimeOnly localTime)
        {
            return !IsClosed
                && localTime >= OpeningTime
                && localTime < OrderCutoffTime;
        }
    }
}