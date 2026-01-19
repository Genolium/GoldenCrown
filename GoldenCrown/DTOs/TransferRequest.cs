using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs
{
    public class TransferRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string ReceiverLogin { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "Сумма должна быть больше нуля")]
        public decimal Amount { get; set; }
    }
}
