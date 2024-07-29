using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATMWebApp.Models
{
    public class User
    {
        public User() { }

        public int ID { get; set; }

        public string? Name { get; set; }

        public string? CardNumber { get; set; }

        [DataType(DataType.Password)]
        [DisplayFormat(DataFormatString = "****", ApplyFormatInEditMode = true)]
        public string? PIN { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }

    }
}
