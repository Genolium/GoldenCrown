namespace GoldenCrown.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid? SenderId { get; set; }
        public User? Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
