using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Inventory.DataAccess.Database
{
    internal static class SqlConnectionFactory
    {
        private static readonly IConfiguration Configuration =
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

        private static readonly string ConnectionString =
            Configuration.GetConnectionString("InventoryDatabase")
            ?? throw new InvalidOperationException(
                "The InventoryDatabase connection string was not found.");

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}