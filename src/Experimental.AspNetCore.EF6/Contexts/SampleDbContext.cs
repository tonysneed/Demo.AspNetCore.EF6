using System.Data.Entity;
using Experimental.AspNetCore.EF6.Models;

namespace Experimental.AspNetCore.EF6.Contexts
{
    [DbConfigurationType(typeof(DbConfig))]
    public class SampleDbContext : DbContext
    {
        static SampleDbContext()
        {
            Database.SetInitializer(new SampleDbInitializer());
        }

        public SampleDbContext(string connectionName) :
            base(connectionName) { }

        public DbSet<Product> Products { get; set; }
    }
}
