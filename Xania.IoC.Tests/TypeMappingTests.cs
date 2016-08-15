using System;
using FluentAssertions;
using NUnit.Framework;

namespace Xania.IoC.Tests
{
	public class TypeMappingTests
	{
		[TestCase(typeof(IProductService), typeof(ProductService), typeof(ProductService))]
		[TestCase(typeof(IRepository<int>), typeof(MemoryRepository<>), typeof(MemoryRepository<int>))]
		[TestCase(typeof(IMap<int, int>), typeof(Map<,>), typeof(Map<int, int>))]
		[TestCase(typeof(IMap<int, double>), typeof(IntKeyMap<>), typeof(IntKeyMap<double>))]
		[TestCase(typeof(IMap<int, double>), typeof(IntKeySubMap<>), typeof(IntKeySubMap<double>))]
        [TestCase(typeof(MemoryRepository<int>), typeof(MemoryRepository<>), typeof(MemoryRepository<int>))]
        [TestCase(typeof(TypeMappingTests), typeof(MemoryRepository<>), null)]
        public void GenericType_mapping(Type targetType, Type templateType, Type concreteType)
		{
			templateType.MapTo(targetType).Should().BeSameAs(concreteType);
		}

		private class IntKeyMap<T> : IMap<int, T>
		{

		}

		private class IntKeySubMap<T> : IntKeyMap<T>
		{
		}
	}
}
