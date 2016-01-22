using System.Data.Entity;
using Experimental.AspNetCore.EF6.Models;

namespace Experimental.AspNetCore.EF6.Data
{
    [DbConfigurationType(typeof(DbConfig))]
    public class SampleContext : DbContext
    {
        public SampleContext(string connectionName) :
            base(connectionName) { }

        public DbSet<Product> Products { get; set; }
    }
}
