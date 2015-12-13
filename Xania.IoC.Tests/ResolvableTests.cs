using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Xania.IoC.Tests
{
    public class ResolvableTests
    {
        [Test]
        public void Resolver_returns_null_for_unknown_type()
        {
            new Resolver().Resolve<ProductService>().Should().BeNull();
        }

        [Test]
        public void Resolver_returns_instance()
        {
            new Resolver()
                .Register<ProductService>()
                .Register<DataContext>()
                .Resolve<ProductService>().Should().NotBeNull();
        }

        [Test]
        public void Resolver_throws_when_unable_to_resolve_underlying_dependencies()
        {
            new Resolver()
                .Register<ProductService>()
                .Invoking(c => c.Resolve<ProductService>())
                .ShouldThrow<ResolutionFailedException>();
        }
    }

}
