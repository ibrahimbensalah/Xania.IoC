﻿using FluentAssertions;
using NUnit.Framework;

namespace Xania.IoC.Tests
{
    public class ResolvableTests
    {
        [Test]
        public void Resolver_throws_when_resolving_unregistered_interface()
        {
            new Resolver()
                .Invoking(c => c.Resolve<IDataContext>())
                .ShouldThrow<ResolutionFailedException>();
        }

        [Test]
        public void Resolver_returns_instance()
        {
            var resolver = new Resolver()
                .Register<ProductService>()
                .Register<DataContext>();

            resolver
                .Resolve<IProductService>()
                .Should().NotBeNull();
        }

        [Test]
        public void Resolver_throws_when_unable_to_resolve_underlying_dependencies()
        {
            new Resolver()
                .Register<ProductService>()
                .Invoking(c => c.Resolve<ProductService>())
                .ShouldThrow<ResolutionFailedException>();
        }

        [Test]
        public void ResolveCollection_resolves_instance()
        {
            var resolver = new ResolverCollection
            {
                new Resolver().Register<DataContext>(),
                new Resolver().Register<ProductService>()
            };

            resolver.Resolve<IProductService>().Should().BeOfType<ProductService>();
        }

        [Test]
        public void PerTypeContainer_resolves_same_instance_from_type()
        {
            var container = new CachedObjectContainer(new ConventionBasedResolver());

            var instance1 = container.Resolve<DataContext>();
            var instance2 = container.Resolve<DataContext>();

            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void TrantientObjectController_resolves_nonequal_instances()
        {
            var container = new TransientObjectContainer(new Resolver().Register<DataContext>());

            var instance1 = container.Resolve<DataContext>();
            var instance2 = container.Resolve<DataContext>();

            instance1.Should().NotBe(instance2);
        }
    }
}
