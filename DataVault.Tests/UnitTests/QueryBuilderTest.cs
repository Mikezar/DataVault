using DataVault.Storage.Common;
using DataVault.Storage.Core.QueryBuilder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class QueryBuilderTest
    {
        private class TestEntity
        {
            public string Description { get; set; }
        }

        private class TestEntity2
        {
            [SCI]
            public int Id { get; set; }
            public string Description { get; set; }
        }

        [Test]
        public void QueryBuilder_CreateeQuery_Success()
        {
            var data = new List<TestEntity2>() { new TestEntity2() { Id = 3} };

            IQueryBuilder builder = new QueryBuilder();

            var query = builder.CreateQuery(QueryType.Add, data);


            var ids = ((IEnumerable<int>)query.SystemData["CorrelationId"]);
            var sciName = query.SystemData["SCIName"];
            Assert.AreEqual(data[0].Id, ids.First());
            Assert.AreEqual("Id", sciName);
        }

        [Test]
        public void QueryBuilder_CreateeQuery_Fail()
        {
            var data = new List<TestEntity>() { new TestEntity() { Description = "Test Decription" } };

            IQueryBuilder builder = new QueryBuilder();

            Assert.Throws<InvalidOperationException>(() => builder.CreateQuery(QueryType.Add, data));
        }
    }
}
