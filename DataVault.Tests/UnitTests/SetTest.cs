using DataVault.Storage.Core.Sets;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using DataVault.Storage.Core.Observables;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DataVault.Tests.SetTest
{
    [TestFixture]
    public class SetTest
    {
        private Mock<IObservationCollection<TestEntity>> _observable;

        public class TestEntity
        {
            public string Name { get; set; }
        }

        [SetUp]
        public void Init()
        {
            _observable = new Mock<IObservationCollection<TestEntity>>();

            _observable.Setup(x => x.Collection).Returns(new ObservableCollection<TestEntity>(new List<TestEntity>()
            {
                   new TestEntity() { Name = "Test"}
            }));
        }

        [Test]
        public void DataSet_Add_Success()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            tEntity.Add(new TestEntity() { Name = "Test2" });
     
            var entities = tEntity.Entities;

            Assert.IsNotNull(entities);
            Assert.AreEqual(2, entities.Count());
        }

        [Test]
        public void DataSet_Add_Fail()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            Assert.Throws<NullReferenceException>(() => tEntity.Add(null));
        }

        [Test]
        public async Task DataSet_AddAsync_Success()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            await tEntity.AddAsync(new TestEntity() { Name = "Test2" });

            var entities = tEntity.Entities;

            Assert.IsNotNull(entities);
            Assert.AreEqual(2, entities.Count());
        }

        [Test]
        public void DataSet_Find_Success()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            var entity = new TestEntity() { Name = "Test" };

            tEntity.Add(entity);

            var result = tEntity.Find(entity);

            Assert.IsNotNull(entity);
            Assert.AreSame(entity, result);
        }

        [Test]
        public void DataSet_Find_Fail()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            var result = tEntity.Find(new TestEntity() { Name = "Test" });

            Assert.IsNull(result);
        }

        [Test]
        public async Task DataSet_AddRangeAsync_Success()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            var result = await tEntity.AddRangeAsync(new List<TestEntity>() { new TestEntity() { Name = "Test" }, new TestEntity() { Name = "Test" } });

            Assert.IsNotNull(result);
            Assert.AreEqual(3, tEntity.Entities.Count());
        }


        [Test]
        public void DataSet_Update_Success()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            var entity = tEntity.Entities.First();

            entity.Name = "ChangedName";

            tEntity.Update(entity);

            var result = tEntity.Find(entity);

            Assert.IsNotNull(result);
            Assert.AreEqual(result, entity);
            Assert.AreSame("ChangedName", entity.Name);
        }

        [Test]
        public async Task DataSet_DeleteRangeAsync_Success()
        {
            IDataSet<TestEntity> tEntity = new DataSet<TestEntity>(_observable.Object);

            var list = new List<TestEntity>() { new TestEntity() { Name = "Test" }, new TestEntity() { Name = "Test" } };

            var result = await tEntity.AddRangeAsync(list);

            Assert.AreEqual(3, tEntity.Entities.Count());

            await tEntity.DeleteRangeAsync(list);

            Assert.AreEqual(1, tEntity.Entities.Count());
        }
    }
}
