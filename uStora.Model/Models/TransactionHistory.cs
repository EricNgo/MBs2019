using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uStora.Model.Models
{
    public class TransactionHistory
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Description { get; set; }

        public decimal Money { get; set; }

        public int MoneyInSite { get; set; }
        
        public string CardOwner { get; set; }

        [ForeignKey("CardOwner")]
        public ApplicationUser ApplicationUser { get; set; }

        public TransactionHistory()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
