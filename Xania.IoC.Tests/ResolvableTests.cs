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
        public void ResolveTest()
        {
            new Resolver().Resolve<ProductService>().Should().NotBeNull();
        }
    }

}
