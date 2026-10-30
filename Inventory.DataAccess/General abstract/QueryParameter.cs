using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces_and_abstract.General_abstract
{
    public class QueryParameter
    {
        public string Name { get;  }
        public SqlDbType Type { get;  }
        public object? Value { get; }
        public QueryParameter(string name, SqlDbType type, object? value) { Name = name; Type = type; Value = value; }

    }
}
