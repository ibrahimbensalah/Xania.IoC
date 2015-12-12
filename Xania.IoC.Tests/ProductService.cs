using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.IoC.Tests
{
    public class ProductService: IDisposable, IProductService
    {
        public DataContext Db { get; private set; }

        public ProductService(DataContext db)
        {
            Db = db;
        }

        public void Dispose()
        {
            
        }
    }

    public class DataContext : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
