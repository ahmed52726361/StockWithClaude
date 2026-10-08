using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services
{
    internal interface IRepository<T> where T : IHasId
    {
        void Add(T item);
        T GetById(int id);
        T Find(Predicate<T> predicate);
        List<T> FindAll(Predicate<T> predicate);
    }
}
