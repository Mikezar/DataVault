using DataVault.Storage.Core.Sets;
using DataVault.Storage.Context;
using NUnit.Framework;
using System;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class ContextTest
    {
        private class TestEntity
        {
            public string Name { get; set; }
        }

        private class TestEntity2
        {
            public string Name { get; set; }
        }

        private class TestEntity3
        {
            public string Name { get; set; }
        }

        [Test]
        public void Context_RegisterTest_Success()
        {
            IContext iContext = VaultContext.GetContext();

            var dataSet = iContext.GetDataSet<TestEntity3>();

            Assert.IsNotNull(dataSet);
            Assert.IsInstanceOf(typeof(DataSet<TestEntity3>), dataSet);
        }

        [Test]
        public void Context_RegisterTest_Fail()
        {
            EntityContainer.RegisterEntity<TestEntity>();
            EntityContainer.RegisterEntity<TestEntity2>();

            Assert.Throws<InvalidOperationException>(() => EntityContainer.RegisterEntity<TestEntity2>());
        }

        [Test]
        public void Context_ContextInit_Success()
        {
            EntityContainer.RegisterEntity<TestEntity3>();

            IContext iContext = VaultContext.GetContext();

            Assert.NotNull(iContext);
        }

        [Test]
        public void Context_GetContext_Success()
        {
            var iContext = VaultContext.GetContext();
            var iContext2 = VaultContext.GetContext();

            Assert.NotNull(iContext);
            Assert.NotNull(iContext2);
            Assert.AreSame(iContext, iContext2);
        }
    }
}
