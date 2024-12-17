using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Adv._Project.Data;
using Adv._Project.Models;
using OfficeOpenXml; // For Excel file generation
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Adv._Project.Controllers
{
    [Authorize(Roles = "PremiumUser")] // Restrict access to PremiumUser role
    public class PremiumFeaturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PremiumFeaturesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PremiumFeatures/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: PremiumFeatures/AdvancedAnalytics
        // GET: PremiumFeatures/AdvancedAnalytics
        public async Task<IActionResult> AdvancedAnalytics()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.Id && t.Type == TransactionType.Expense)
                .ToListAsync();

            var monthlyData = transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new TopExpenseViewModel
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    TopCategory = g.GroupBy(t => t.Category)
                                   .OrderByDescending(cg => cg.Sum(t => t.Amount))
                                   .FirstOrDefault()?.Key ?? "Uncategorized",
                    TotalSpent = g.GroupBy(t => t.Category)
                                  .OrderByDescending(cg => cg.Sum(t => t.Amount))
                                  .FirstOrDefault()?.Sum(t => t.Amount) ?? 0
                })
                .ToList();

            // Optional: Log the data (only for debugging purposes)
            foreach (var data in monthlyData)
            {
                Console.WriteLine($"Month: {data.Month}, Top Category: {data.TopCategory}, Total Spent: {data.TotalSpent}");
            }

            return View(monthlyData);
        }




        // GET: PremiumFeatures/DownloadReport
        public async Task<IActionResult> DownloadReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            // Generate Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transactions");
                worksheet.Cells[1, 1].Value = "Date";
                worksheet.Cells[1, 2].Value = "Description";
                worksheet.Cells[1, 3].Value = "Category";
                worksheet.Cells[1, 4].Value = "Amount";
                worksheet.Cells[1, 5].Value = "Type";

                int row = 2;
                foreach (var transaction in transactions)
                {
                    worksheet.Cells[row, 1].Value = transaction.Date.ToShortDateString();
                    worksheet.Cells[row, 2].Value = transaction.Description;
                    worksheet.Cells[row, 3].Value = transaction.Category;
                    worksheet.Cells[row, 4].Value = transaction.Amount;
                    worksheet.Cells[row, 5].Value = transaction.Type.ToString();
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"TransactionsReport-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> BudgetPlanner()
        {
            var user = await _userManager.GetUserAsync(User);
            var expensesThisMonth = _context.Transactions
                .Where(t => t.UserId == user.Id && t.Date.Month == DateTime.Now.Month && t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var model = new BudgetPlannerViewModel
            {
                Budget = user.MonthlyBudget,
                MonthlyExpenses = expensesThisMonth
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BudgetPlanner(decimal budget)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.MonthlyBudget = budget;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("BudgetPlanner");
        }
        
    }
}
