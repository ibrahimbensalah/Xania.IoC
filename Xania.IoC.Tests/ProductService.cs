using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.IoC.Tests
{
    public class ProductService: IDisposable, IProductService
    {
        public IDataContext Db { get; private set; }

        public ProductService(IDataContext db)
        {
            Db = db;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }
    }

    public interface IProductService
    {
        bool IsDisposed { get; }
    }

    public interface IDataContext
    {
        
    }

    public class DataContext : IDataContext, IDisposable
    {
        public void Dispose()
        {
        }
    }
}
