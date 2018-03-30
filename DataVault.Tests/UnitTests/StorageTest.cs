using DataVault.Storage.Common;
using DataVault.Storage.Core.Cache;
using DataVault.Storage.Core.Exceptions;
using DataVault.Storage.Core.Providers;
using DataVault.Storage.Core.QueryBuilder;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class StorageTest
    {
        private Mock<ITextFileProvider> _provider;
        private Mock<ICache> _cache;

        private class TEntity
        {
            [SCI]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void Storage_TryGetFromCache_Success()
        {
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();
            _cache.Setup(x => x.TryGet<TEntity>("VaultDataKey")).Returns(() => new List<TEntity>() { new TEntity() { Name = "Cached" } });
            _cache.Setup(x => x.GetActuality<TEntity>()).Returns(() => true);

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);

            IEnumerable<TEntity> list = new List<TEntity>();

            var result = storage.TryGetFromCache(out list);

            Assert.IsTrue(result);
            Assert.IsTrue(list.Count() == 1);
        }

        [Test]
        public void Storage_TryGetFromCache_Fail()
        {
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();
            _cache.Setup(x => x.TryGet<TEntity>("VaultDataKey")).Returns(() => null);
            _cache.Setup(x => x.GetActuality<TEntity>()).Returns(() => false);

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);

            IEnumerable<TEntity> list = new List<TEntity>();

            var result = storage.TryGetFromCache(out list);

            Assert.IsFalse(result);
            Assert.IsNull(list);
        }

        [Test]
        public void Storage_ReadData_Success()
        {
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();
            _cache.Setup(x => x.TryGet<TEntity>("VaultDataKey")).Returns(() => new List<TEntity>() { new TEntity() { Name = "Cached" } });
            _cache.Setup(x => x.GetActuality<TEntity>()).Returns(() => true);

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);

            var result = storage.ReadData<TEntity>();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public void Storage_ReadData_Success2()
        {
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();       
            _provider.Setup(x => x.Execute<TEntity>(It.IsAny<IQuery>()))
                .Returns(async () => await Task.FromResult((IQueryResult<TEntity>)new QueryResult<TEntity>()
                {
                    Exception = null,
                    Success = true,
                    Result = new List<TEntity>() { new TEntity() { Name = "Cached" } }
                }));

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);

            var result = storage.ReadData<TEntity>();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public void Storage_ReadData_Fail()
        {
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();
            _provider.Setup(x => x.Execute<TEntity>(It.IsAny<IQuery>()))
                .Returns(async () => await Task.FromResult((IQueryResult<TEntity>)new QueryResult<TEntity>()
                {
                    Exception = new InvalidOperationException(),
                    Success = false,
                    Result = null 
                }));

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);;

            Assert.Throws<VaultTransactionException>(() => storage.ReadData<TEntity>());
        }

        [Test]
        public void Storage_ManipulateData_Success()
        {

            var data = new List<TEntity>() { new TEntity() { Name = "Cached" } };
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();
            _provider.Setup(x => x.Execute<TEntity>(It.IsAny<IQuery>()))
                .Returns(async () => await Task.FromResult((IQueryResult<TEntity>)new QueryResult<TEntity>()
                {
                    Exception = null,
                    Success = true,
                    Result = null
                }));

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);

            Assert.DoesNotThrow(() => storage.ManipulateData(QueryType.Add, data));
        }


        [Test]
        public void Storage_ManipulateData_Fail()
        {
            var data = new List<TEntity>() { new TEntity() { Name = "Cached" } };
            _cache = new Mock<ICache>();
            _provider = new Mock<ITextFileProvider>();
            _provider.Setup(x => x.Execute<TEntity>(It.IsAny<IQuery>()))
                .Returns(async () => await Task.FromResult((IQueryResult<TEntity>)new QueryResult<TEntity>()
                {
                    Exception = new Exception("Test"),
                    Success = false,
                    Result = null
                }));

            var storage = new Storage.Core.Storage.Storage(_provider.Object, _cache.Object);

            Assert.Throws<StorageException>(() => storage.ManipulateData(QueryType.Add, data));
        }
    }
}
