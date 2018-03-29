using DataVault.Storage.Core.Views;
using NUnit.Framework;
using System.Collections.Generic;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class ViewTest
    {
        public class TestEntity
        {
            public string Description { get; set; }
        }

        [Test]
        public void BindView_Success()
        {
            var list = new List<TestEntity>()
            {
                new TestEntity() { Description = "Test1" },
                new TestEntity() { Description = "Test2" }
            };

            IView view = new View();

            view.Bind(list);

            Assert.IsTrue(view.UpToDate);
            Assert.IsTrue(view.Exists);

        }

        [Test]
        public void RestoreView_Success()
        {
            var list = new List<TestEntity>()
            {
                new TestEntity() { Description = "Test1" },
                new TestEntity() { Description = "Test2" }
            };

            IView view = new View();

            view.Bind(list);

            var result = view.Restore();

            Assert.IsNotNull(result);
            Assert.AreNotSame(result, list);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
