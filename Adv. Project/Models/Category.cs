using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Adv._Project.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string UserId { get; set; } // Nullable for default categories

        public virtual IdentityUser User { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
