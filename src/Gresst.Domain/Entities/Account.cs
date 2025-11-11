namespace Gresst.Domain.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; } // e.g., "S" for logistic operator, "N" for waste generator
        public string PersonId { get; set; }
        public long AdministratorId { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public User Administrator { get; set; }
    }
}
