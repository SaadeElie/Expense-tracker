using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Adv._Project.Data;
using Adv._Project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Adv._Project.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            System.Diagnostics.Debug.WriteLine("IM TRYING TO PRINT SOMETHING AAAAAAAAAAAAAAAAAAAAAAAAAA");
            // Ensure transactions are not null
            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.Id)
                .ToListAsync() ?? new List<Transaction>();

            // Defensive programming for calculations
            var totalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .DefaultIfEmpty()
                .Sum(t => t.Amount);

            var totalExpenses = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .DefaultIfEmpty()
                .Sum(t => t.Amount);

            // Safer savings rate calculation
            var savingsRate = totalIncome > 0
                ? Math.Round(Math.Max(0, totalIncome - totalExpenses) / totalIncome * 100, 2)
                : 0;

            // Ensure these are never null
            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList() ?? new List<Transaction>();

            var expensesByCategory = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category ?? "Uncategorized")
                .Select(g => new CategoryExpense
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                    Percentage = totalExpenses > 0
                        ? Math.Round(g.Sum(t => t.Amount) / totalExpenses * 100, 2)
                        : 0
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToList() ?? new List<CategoryExpense>();

            var monthlyFinancials = transactions
                .GroupBy(t => new {
                    Year = t.Date.Year,
                    Month = t.Date.Month
                })
                .Select(g => new MonthlyFinancial
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderByDescending(m => m.Month)
                .Take(6)
                .ToList() ?? new List<MonthlyFinancial>();

            var viewModel = new DashboardViewModel
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                SavingsRate = savingsRate,
                ExpensesByCategory = expensesByCategory,
                RecentTransactions = recentTransactions,
                MonthlyFinancials = monthlyFinancials
            };

            return View("~/Views/Home/Index.cshtml", viewModel);
        }
    }
}