using System.Data;

namespace Inventory.DataAccess.General_abstract
{
    public class QueryParameter
    {
        public string Name { get;  }
        public SqlDbType Type { get;  }
        public object? Value { get; }
        public QueryParameter(string name, SqlDbType type, object? value) { Name = name; Type = type; Value = value; }

    }
}
