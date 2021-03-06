﻿using System;

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

    public interface IDataContext: IDisposable
    {
        bool IsDisposed { get; }
        Guid Id { get; }
    }

    public class DataContext : IDataContext, IDisposable
    {
        public DataContext()
        {
            Id = Guid.NewGuid();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public Guid Id { get; private set; }

        public bool IsDisposed { get; private set; }
    }

    public interface IRepository<T>
    {
        
    }

    public class MemoryRepository<T> : IRepository<T>
    {
    }

    public class IntegerRepository : MemoryRepository<int>
    {
    }
}
