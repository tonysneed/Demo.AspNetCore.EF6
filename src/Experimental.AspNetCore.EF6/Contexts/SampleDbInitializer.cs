using System.Collections.Generic;
using System.Data.Entity;
using Experimental.AspNetCore.EF6.Models;

namespace Experimental.AspNetCore.EF6.Contexts
{
    public class SampleDbInitializer : DropCreateDatabaseIfModelChanges<SampleDbContext>
    {
        protected override void Seed(SampleDbContext context)
        {
            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Chai", UnitPrice = 10 },
                new Product { Id = 2, ProductName = "Chang", UnitPrice = 11 },
                new Product { Id = 3, ProductName = "Aniseed Syrup", UnitPrice = 12 },
            };

            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }
}
