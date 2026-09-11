namespace DMendez.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        public Category (string name)
        {
            Id = Guid.NewGuid();
            name = ValidateName(Name);
            IsActive = true;
        }

        public void Rename(string name)
        {
            Name = ValidateName(name);
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Desactive()
        {
            IsActive = false;  
        }

        private static string ValidateName (string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre de la categoria es obligatorio", 
                    nameof(name));
            }

            return name.Trim();
        }
    }
}