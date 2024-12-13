using Adv._Project.Data;
using Adv._Project.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Adv._Project.Seeders
{
    public static class CategorySeeder
    {
        public static void SeedCategories(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Ensure the database is created
                context.Database.EnsureCreated();

                // Check if categories already exist
                if (!context.Categories.Any())
                {
                    // Add default categories
                    context.Categories.AddRange(
                            new Category { Name = "Rent", UserId = null },
                            new Category { Name = "Utilities", UserId = null },
                            new Category { Name = "Groceries", UserId = null },
                            new Category { Name = "Transportation", UserId = null }
                    );

                    // Save changes to the database
                    context.SaveChanges();
                }
            }
        }
    }
}
