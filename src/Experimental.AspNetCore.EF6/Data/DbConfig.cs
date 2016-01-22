using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Experimental.AspNetCore.EF6.Data
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
        }
    }
}
