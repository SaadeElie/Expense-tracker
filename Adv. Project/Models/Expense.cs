using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adv._Project.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // FK to Identity User

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int CategoryId { get; set; } // FK to Category

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public string Currency { get; set; }
    }
}
