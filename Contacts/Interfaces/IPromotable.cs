using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory
{
    internal interface IPromotable
    {
        bool AcceptsPromotions();
        string Name {  get; }   
    }
}
