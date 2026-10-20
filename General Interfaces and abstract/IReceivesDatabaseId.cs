using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.General_Interfaces
{
    internal interface IReceivesDatabaseId : IHasId
    {
        void SetId(int id);
    }
}
