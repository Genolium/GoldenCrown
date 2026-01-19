namespace GoldenCrown.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
