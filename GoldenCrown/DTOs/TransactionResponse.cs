namespace GoldenCrown.DTOs
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string Type { get; set; }
    }
}
