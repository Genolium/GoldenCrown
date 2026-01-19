using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.DTOs
{
    public class TransactionHistoryRequest
    {
        [FromQuery(Name = "token")] 
        public string Token { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
    }
}
