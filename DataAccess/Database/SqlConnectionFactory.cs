using Microsoft.Data.SqlClient;

namespace Inventory.DataAccess.Database
{
    internal static class SqlConnectionFactory
    {
        private const string ConnectionString =
            "Server=AHMED1-HP0;Database=InventoryWithClaude;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}