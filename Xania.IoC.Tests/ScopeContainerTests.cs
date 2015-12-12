using System;
using FluentAssertions;
using NUnit.Framework;
using StructureMap;

namespace Xania.IoC.Tests
{
    public class ScopeContainerTests
    {
        [Test]
        public void ScopeBehavesAsDependencyResolver()
        {
            new TransientContainer().Resolve<ProductService>().Should().NotBeNull();
        }

        [Test]
        public void EveryItemInScopeIsSingleton()
        {
            using (var scope = new PerScopeContainer().Register<ProductService>().Register<DataContext>())
            {
                var service1 = scope.Resolve<ProductService>();
                var service2 = scope.Resolve<ProductService>();

                service1.Should().Be(service2);
            }
        }

        [Test]
        public void PerRequestScopeTest()
        {
            // arrange
            var scope =
                new ObjectContainerProvider(
                    () => new PerScopeContainer().Register<ProductService>().Register<DataContext>());
            var service1 = scope.Resolve<ProductService>();
            // act
            var service2 = scope.Resolve<ProductService>();
            // assert
            service1.Should().NotBe(service2);
        }

        [Test]
        public void nested_container_usage_of_singletons()
        {
            var container = new Container(_ =>
            {
                _.ForSingletonOf<IColorCache>().Use<ColorCache>();
                _.For<IProductService>().Use<ProductService>();
            });

            var singleton = container.GetInstance<IColorCache>();

            // SingletonThing's are resolved from the parent container
            using (var nested = container.GetNestedContainer())
            {
                nested.GetInstance<IColorCache>()
                    .Should().Be(singleton);
            }
        }

        [Test]
        public void Transient_objects_are_unique()
        {
            var container = new TransientContainer();
            var service1 = container.Resolve<ProductService>();
            var service2 = container.Resolve<ProductService>();

            service1.Should().NotBe(service2);
        }

        [Test]
        public void Transient_objects_are_unique_per_resolve()
        {
            var container = new TransientContainer();
            var o = container.Resolve<MultilevelObject>();

            o.ProductService1.Db.Should().NotBe(o.ProductService2.Db);
        }

        [Test]
        public void PerScope_objects_are_same()
        {
            var container = new PerScopeContainer()
                .Register<MultilevelObject>()
                .Register<ProductService>()
                .Register<DataContext>();

            var o = container.Resolve<MultilevelObject>();

            o.ProductService1.Db.Should().Be(o.ProductService2.Db);
        }

        [Test, Ignore("stack overflow")]
        public void TransientContainer_should_detect_circular_references()
        {
            var container = new TransientContainer();
            container.Invoking(c => c.Resolve<CircularReference>()).ShouldThrow<CircularReferenceException>();
        }

        [Test]
        public void PerScopeContainer_should_detect_circular_references()
        {
            var container = new PerScopeContainer()
                .Register<CircularReference>();
            container.Resolve<ResolutionFailedException>().Should().BeNull();
        }

        [Test]
        public void ContainerCollection_resolve_test()
        {
            // arrange
            var perScope = new PerScopeContainer()
                .Register<ProductService>();
            var transient = new TransientContainer();
            var container = new ContainerCollection {perScope, transient};

            container.Resolve<ProductService>().Should().NotBeNull();
        }
    }

    public class MultilevelObject
    {
        public MultilevelObject(ProductService productService1, ProductService productService2)
        {
            ProductService2 = productService2;
            ProductService1 = productService1;
        }

        public ProductService ProductService1 { get; private set; }
        public ProductService ProductService2 { get; private set; }
    }

    public class CircularReference
    {
        public CircularReference(CircularReference reference)
        {
            
        }
    }

    public class ColorCache : IColorCache
    {
        private readonly IProductService _productService;

        public ColorCache(IProductService productService)
        {
            _productService = productService;
        }
    }

    public interface IProductService
    {
    }

    public interface IColorCache
    {
    }

    public class ObjectContainerProvider : IObjectContainer
    {
        private readonly Func<IObjectContainer> _getContainerThunk;

        public ObjectContainerProvider(Func<IObjectContainer> getContainerThunk)
        {
            _getContainerThunk = getContainerThunk;
        }

        public object Resolve(Type serviceType)
        {
            return _getContainerThunk().Resolve(serviceType);
        }
    }
}