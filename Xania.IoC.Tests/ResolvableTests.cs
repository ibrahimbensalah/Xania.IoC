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
            var resolver = new ContainerControlledResolver(new TransientResolver());

            resolver.ResolveAll(typeof(IProductService)).Should().BeEmpty();
        }

        [Test]
        public void Resolver_returns_instance()
        {
            var resolver = new TransientResolver()
                .Register<ProductService>()
                .Register<DataContext>();

            resolver
                .Resolve<IProductService>()
                .Should().NotBeNull();
        }

        [Test]
        public void Resolver_throws_when_unable_to_resolve_underlying_dependencies()
        {
            new TransientResolver()
                .Register<ProductService>()
                .Invoking(c => c.Resolve<ProductService>())
                .ShouldThrow<ResolutionFailedException>();
        }

        [Test]
        public void ResolveCollection_resolves_instance()
        {
            var resolver = new ResolverCollection
            {
                new TransientResolver().Register<DataContext>(),
                new TransientResolver().Register<ProductService>()
            };

            resolver.Resolve<IProductService>().Should().BeOfType<ProductService>();
        }

        [Test]
        public void PerTypeContainer_resolves_same_instance_from_type()
        {
            var container = new ContainerControlledResolver(new ConventionBasedResolver());

            var instance1 = container.Resolve<DataContext>();
            var instance2 = container.Resolve<DataContext>();

            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void TransientObjectController_resolves_nonequal_instances()
        {
            var container = new TransientResolver().Register<DataContext>();

            var instance1 = container.Resolve<DataContext>();
            var instance2 = container.Resolve<DataContext>();

            instance1.Should().NotBe(instance2);
        }

        [Test]
        public void ConventionBasedResolver_resolves_closest_subtype()
        {
            var resolver = new ResolverCollection()
            {
                // new TransientResolver().Register<ProductService>(),
                new ConventionBasedResolver()
            };

            resolver.Resolve<IProductService>().Should().BeOfType<ProductService>();
            resolver.Resolve<IDataContext>().Should().BeOfType<DataContext>();
        }

        public class SubDataContext: DataContext
        {
        }

        [Test]
        public void DisposeTests()
        {
            var resolver = new ContainerControlledResolver(new ConventionBasedResolver());
            var productService = resolver.Resolve<IProductService>();
            resolver.Resolve<IProductService>().Should().BeSameAs(productService);

            resolver.Dispose(typeof(IProductService));
            productService.IsDisposed.Should().BeTrue();

            resolver.Resolve<IProductService>().Should().NotBeSameAs(productService);
        }

        [Test]
        public void PerScopeResolver_should_resolve_proxy_that_propagate_methodcall_to_object_in_scope()
        {
            var scopeProvider = new ScopeProvider();
            var resolver = new PerScopeResolver(scopeProvider, new ConventionBasedResolver());

            var proxy = resolver.Resolve<IDataContext>();
            var id = proxy.Id;
            proxy.Id.Should().Be(id);

            // act, change scope
            proxy.Dispose();

            // resolve after scope change
            proxy.Id.Should().NotBe(id);
        }

        [Test]
        public void Should_be_able_to_resolve_subtypes()
        {
            var resolver = new IdentityResolver().For<IDataContext>();

            resolver.Resolve<DataContext>().Should().BeOfType<DataContext>();
        }

        [Test]
        public void Should_be_able_to_resolve_generics()
        {
            var resolver = new IdentityResolver().For(typeof (MemoryRepository<>));
            resolver.Resolve<IntegerRepository>().Should().BeOfType<IntegerRepository>();
        }

    }
}
