using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adv._Project.Models
{
    public enum TransactionType
    {
        Income,
        Expense
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(100)]
        public string Category { get; set; }
    }
}