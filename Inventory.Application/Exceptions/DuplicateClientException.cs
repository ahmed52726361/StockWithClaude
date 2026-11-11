using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Exceptions
{
    public class DuplicateClientException(int ClinetId):Exception($"Client with ID:{ClinetId} already excist")
    {
    }
}
