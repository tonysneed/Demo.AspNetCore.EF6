using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Experimental.AspNetCore.EF6.Contexts
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
        }
    }
}
