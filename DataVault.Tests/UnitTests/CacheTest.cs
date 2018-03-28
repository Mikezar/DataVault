using DataVault.Storage.Core.Cache;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class CacheTest
    {
        private const string _region = "TestRegion";

        public class TestEntity
        {
            public int Count { get; set; }
        }

        [Test]
        public void TryGetCache_Success()
        {
            ICache cache = new Cache();

            cache.Set(new List<TestEntity>() { new TestEntity { Count = 4 } }, _region);

            var result = cache.TryGet<TestEntity>(_region);

            var actuality = cache.GetActuality<TestEntity>();

            Assert.IsTrue(actuality);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public void GetAllCache_Success()
        {
            ICache cache = new Cache();

            cache.Set(new List<TestEntity>() { new TestEntity { Count = 4 }, new TestEntity { Count = 7 } }, _region);

            var result = cache.GetAll();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Values.Count() > 0);
        }

        [Test]
        public void GetSetActualityCache_Success()
        {
            ICache cache = new Cache();

            cache.Set(new List<TestEntity>() { new TestEntity { Count = 4 }}, _region);

            cache.SetActuality<TestEntity>(false);

            var result = cache.GetActuality<TestEntity>();

            Assert.IsFalse(result);
        }

        [Test]
        public void ResetCache_Success()
        {
            ICache cache = new Cache();

            cache.Set(new List<TestEntity>() { new TestEntity { Count = 4 } }, _region);

            var result = cache.TryGet<TestEntity>(_region);

            Assert.IsTrue(result.Count() > 0);

            cache.Reset(_region);

            result = cache.TryGet<TestEntity>(_region);

            Assert.IsNull(result);
        }

        [Test]
        public void RemoveCacheEntry_Success()
        {
            ICache cache = new Cache();

            cache.Set(new List<TestEntity>() { new TestEntity { Count = 4 } }, _region);

            var result = cache.TryGet<TestEntity>(_region);

            Assert.IsTrue(result.Count() > 0);

            cache.Remove(typeof(TestEntity).Name, _region);

            result = cache.TryGet<TestEntity>(_region);

            Assert.IsNull(result);
        }
    }
}
