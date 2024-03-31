using System.ComponentModel.DataAnnotations;

namespace ATMWebApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Amount must be between 1 and 1000")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public User User { get; set; }

    }
}
