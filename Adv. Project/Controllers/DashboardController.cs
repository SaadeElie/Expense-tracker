using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Adv._Project.Data;
using Adv._Project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Adv._Project.Services;

namespace Adv._Project.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CurrencyService _currencyService;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            CurrencyService currencyService)
        {
            _context = context;
            _userManager = userManager;
            _currencyService = currencyService;
        }

        [Authorize]
        public async Task<IActionResult> Index(string timeRange, string currency = "USD")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            System.Diagnostics.Debug.WriteLine("IM TRYING TO PRINT SOMETHING AAAAAAAAAAAAAAAAAAAAAAAAAA");

            // Fetch all transactions for the user
            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.Id)
                .ToListAsync() ?? new List<Transaction>();

            // Apply time range filtering if specified
            DateTime now = DateTime.Now;
            if (timeRange == "ThisMonth")
            {
                transactions = transactions
                    .Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month)
                    .ToList();
            }
            else if (timeRange == "ThisYear")
            {
                transactions = transactions
                    .Where(t => t.Date.Year == now.Year)
                    .ToList();
            }

            // Convert transaction amounts if a different currency is selected
            if (currency != "USD")
            {
                for (int i = 0; i < transactions.Count; i++)
                {
                    transactions[i].Amount = await _currencyService.ConvertCurrencyAsync(transactions[i].Amount, "USD", currency);
                }
            }

            // Calculate total income and expenses
            var totalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Select(t=>t.Amount)
                .DefaultIfEmpty(0)
                .Sum();

            var totalExpenses = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Select(t => t.Amount)
                .DefaultIfEmpty(0)
                .Sum();

            // Calculate savings rate
            var savingsRate = totalIncome > 0
                ? Math.Round(Math.Max(0, totalIncome - totalExpenses) / totalIncome * 100, 2)
                : 0;

            // Fetch recent transactions
            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList() ?? new List<Transaction>();

            // Group expenses by category
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

            // Group transactions by month
            var monthlyFinancials = transactions
                .GroupBy(t => new
                {
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

            // Prepare the view model
            var viewModel = new DashboardViewModel
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                SavingsRate = savingsRate,
                ExpensesByCategory = expensesByCategory,
                RecentTransactions = recentTransactions,
                MonthlyFinancials = monthlyFinancials
            };

            ViewBag.SelectedCurrency = currency; // Pass the selected currency to the view
            return View("~/Views/Home/Index.cshtml", viewModel);
        }
    }
}
