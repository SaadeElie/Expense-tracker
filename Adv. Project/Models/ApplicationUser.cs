using Microsoft.AspNetCore.Identity;

namespace Adv._Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public decimal MonthlyBudget { get; set; } = 0; 
    }
}
