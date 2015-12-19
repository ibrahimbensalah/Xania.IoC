﻿using FluentAssertions;
using NUnit.Framework;
using Xania.IoC.Containers;
using Xania.IoC.Resolvers;

namespace Xania.IoC.Tests
{
    public class ResolvableTests
    {
        [Test]
        public void Resolver_throws_when_resolving_unregistered_interface()
        {
            new TransientResolver()
                .Invoking(c => c.Resolve<IDataContext>())
                .ShouldThrow<ResolutionFailedException>();
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
            var container = new ObjectContainer(new ContainerControlledResolver(new ConventionBasedResolver()));

            var instance1 = container.Resolve<DataContext>();
            var instance2 = container.Resolve<DataContext>();

            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void TrantientObjectController_resolves_nonequal_instances()
        {
            var container = new ObjectContainer(new TransientResolver().Register<DataContext>());

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
        public void LifetimeTests()
        {
            // arrange
            var instanceScopeProvider = new ScopeProvider();
            var resolver = new ContainerControlledResolver(new ConventionBasedResolver());
            resolver.AddScopeProvider(typeof (IDataContext), instanceScopeProvider);

            var id = resolver.Resolve<IDataContext>().Id;
            resolver.Resolve<IDataContext>().Id.Should().Be(id);

            // act, change scope
            instanceScopeProvider.Dispose();

            // resolve after scope change
            resolver.Resolve<IDataContext>().Id.Should().NotBe(id);
        }
    }
}
