using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs
{
    public class DepositRequest
    {
        [Required]
        public string Token { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "Сумма должна быть больше нуля")]
        public decimal Amount { get; set; }
    }
}
