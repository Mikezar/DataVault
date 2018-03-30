using DataVault.Storage.Core.MetaData;
using DataVault.Storage.IO;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class MetaHandlerTest
    {
        private Mock<IFileReader> _reader;
        private Mock<IFileWriter> _writer;

        private class TestEntity
        {
           public int Id { get; set; }
           public string Description { get; set; }
           public decimal Price { get; set; }
           public Guid Identity { get; set; }
           public DateTimeOffset DateTime { get; set; }
           public int? Count { get; set; }      
        }

        [SetUp]
        public void Init()
        {
            _reader = new Mock<IFileReader>();
            _writer = new Mock<IFileWriter>();
        }

        [Test]
        public void Meta_GenerateMetaDataFor_Success()
        {
            IMetaHandler handler = new MetadaHandler(_reader.Object, _writer.Object);

            var meta = handler.GenerateMetadataFor<TestEntity>();

            Assert.IsNotNull(meta);
            Assert.AreEqual("Id", meta.Metas.First().FieldName);
        }

        [Test]
        public void Meta_GenerateMetaDataFor_Success2()
        {
            IMetaHandler handler = new MetadaHandler(_reader.Object, _writer.Object);

            var meta = handler.GenerateMetadataFor(typeof(TestEntity),2, new List<MetaList>() {
                new MetaList()
                {
                    Created = DateTimeOffset.Now,
                    Version = 1,
                    Metas = new List<Metadata>()
                    {
                        new Metadata()
                        {
                            FieldName = "Id"                 
                        }
                    }
                }
            });

            Assert.IsNotNull(meta);
            Assert.AreEqual(2, meta.Version);
        }
    }
}
