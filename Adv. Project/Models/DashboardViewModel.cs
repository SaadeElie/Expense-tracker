using System;
using System.Collections.Generic;
using System.Linq;

namespace Adv._Project.Models
{
    public class DashboardViewModel
    {
        public decimal TotalIncome { get; set; } = 0;
        public decimal TotalExpenses { get; set; } = 0;
        public decimal SavingsRate { get; set; } = 0;
        public List<Transaction> RecentTransactions { get; set; } = new List<Transaction>();
        public List<CategoryExpense> ExpensesByCategory { get; set; } = new List<CategoryExpense>();
        public List<MonthlyFinancial> MonthlyFinancials { get; set; } = new List<MonthlyFinancial>();
    }
}