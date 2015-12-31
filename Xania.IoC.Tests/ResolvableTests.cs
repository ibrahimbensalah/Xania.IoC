using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xania.IoC.Containers;
using Xania.IoC.Resolvers;

namespace Xania.IoC.Tests
{
	public class ResolvableTests
	{
		[Test]
		public void Resolver_returns_null()
		{
			var resolver = new ContainerControlledResolver(new RegistryResolver());

			resolver.ResolveAll(typeof(IProductService)).Should().BeEmpty();
		}

		[Test]
		public void Resolver_returns_instance()
		{
			var resolver = new RegistryResolver()
				.Register<ProductService>()
				.Register<DataContext>();

			resolver
				.GetService<IProductService>()
				.Should().NotBeNull();
		}

		[Test]
		public void Resolver_throws_when_unable_to_resolve_underlying_dependencies()
		{
			new RegistryResolver()
				.Register<ProductService>()
				.Invoking(c => c.GetService<ProductService>())
				.ShouldThrow<ResolutionFailedException>();
		}

		[Test]
		public void ResolveCollection_resolves_instance()
		{
			var resolver = new ResolverCollection
			{
				new RegistryResolver().Register<DataContext>(),
				new RegistryResolver().Register<ProductService>()
			};

			resolver.GetService<IProductService>().Should().BeOfType<ProductService>();
		}

		[Test]
		public void PerTypeContainer_resolves_same_instance_from_type()
		{
			var container = new ContainerControlledResolver(new ConventionBasedResolver());

			var instance1 = container.GetService<DataContext>();
			var instance2 = container.GetService<DataContext>();

			instance1.Should().BeSameAs(instance2);
		}

		[Test]
		public void TransientObjectController_resolves_nonequal_instances()
		{
			var container = new RegistryResolver().Register<DataContext>();

			var instance1 = container.GetService<DataContext>();
			var instance2 = container.GetService<DataContext>();

			instance1.Should().NotBe(instance2);
		}

		[Test]
		public void ConventionBasedResolver_resolves_closest_subtype()
		{
			var resolver = new ResolverCollection()
			{
				new ConventionBasedResolver()
			};

			resolver.GetService<IProductService>().Should().BeOfType<ProductService>();
			resolver.GetService<IDataContext>().Should().BeOfType<DataContext>();
		}

		public class SubDataContext : DataContext
		{
		}

		[Test]
		public void DisposeTests()
		{
			var resolver = new ContainerControlledResolver(new ConventionBasedResolver());
			var productService = resolver.GetService<IProductService>();
			resolver.GetService<IProductService>().Should().BeSameAs(productService);

			resolver.Dispose(typeof(IProductService));
			productService.IsDisposed.Should().BeTrue();

			resolver.GetService<IProductService>().Should().NotBeSameAs(productService);
		}

		[Test]
		public void PerScopeResolver_should_resolve_proxy_that_propagate_methodcall_to_object_in_scope()
		{
			var scopeProvider = new ScopeProvider();
		    var resolver = new PerScopeResolver(scopeProvider) {new ConventionBasedResolver()};

			var proxy = resolver.GetService<IDataContext>();
			var id = proxy.Id;
			proxy.Id.Should().Be(id);

			// act, change scope
			proxy.Dispose();

			// resolve after scope change
			proxy.Id.Should().Be(id);
		    proxy.IsDisposed.Should().BeTrue();
		}

		[Test]
		public void Should_be_able_to_resolve_subtypes()
		{
			var resolver = new IdentityResolver().For<IDataContext>();

			resolver.GetService<DataContext>().Should().BeOfType<DataContext>();
		}

		[Test]
		public void Should_be_able_to_resolve_generics()
		{
			var resolver = new IdentityResolver().For(typeof(MemoryRepository<>));
			resolver.GetService<IntegerRepository>().Should().BeOfType<IntegerRepository>();
		}

	    [Test]
	    public void Should_throw_resolution_failed_exception_when_dependency_in_outer_scope()
	    {
	        var registryResolver = new RegistryResolver().Register<DataContext>();
	        var resolver = new PerScopeResolver // per request
	        {
                registryResolver,
	            new PerScopeResolver // per session
	            {
                    new RegistryResolver()
                        .Register(new DataContextAdapter(() => registryResolver.GetService<IDataContext>()))
                        .Register<ProductService>(),
	            }
	        };

            var db = resolver.GetService<ProductService>().Db;
            db.Should().BeOfType<DataContextAdapter>();

	        resolver.GetService<ProductService>().Db.Should().Be(db);
            resolver.GetService<IDataContext>().Should().Be(db);
	    }

		[Test]
		public void Should_report_resolve_path_on_error()
		{
			var resolver = new RegistryResolver()
				.Register<ProductService>()
				.Register<ProductController>();

			try
			{
				resolver.GetService<ProductController>();
				Assert.Fail();
			}
			catch (ResolutionFailedException ex)
			{
				ex.Types.Should().BeEquivalentTo(typeof(IDataContext), typeof(ProductService), typeof(ProductController));
			}
		}
	}

    public class DataContextAdapter: IDataContext
    {
        private readonly Func<IDataContext> _dataContextFunc;

        public DataContextAdapter(Func<IDataContext> dataContextFunc)
        {
            _dataContextFunc = dataContextFunc;
        }

        public void Dispose()
        {
            _dataContextFunc().Dispose();
        }

        public bool IsDisposed
        {
            get { return _dataContextFunc().IsDisposed; }
        }

        public Guid Id
        {
            get { return _dataContextFunc().Id; }
        }
    }

    public class ProductController
	{
		public ProductController(ProductService productService)
		{

		}
	}

	public class Map<T, T1> : IMap<T, T1>
	{
	}

	public interface IMap<T, T1>
	{
	}
}
