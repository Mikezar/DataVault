using DataVault.Storage.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVault.Tests.UnitTests
{
    [TestFixture]
    public class InputOutputTest
    {
        private class TestEntity
        {
            public string Name => "TestEntity";
        }

        [Test]
        public void IO_GetAppPath_Success()
        {
            var path = IOHelper.GetAppPath();

            Assert.IsNotEmpty(path);
            Assert.IsTrue(path.EndsWith("DataVault.Tests"));
        }

        [Test]
        public void IO_CreateDirectoryIfNotExists_Success()
        {
            var directoryPath = IOHelper.CreateDirectoryIfNotExists();

            var path = IOHelper.GetAppPath();

            Assert.IsNotEmpty(directoryPath);
            Assert.AreEqual(Path.Combine(path, "VaultData"), directoryPath);
            Assert.IsTrue(Directory.Exists(directoryPath));
        }

        [Test]
        public void IO_CreateFileIfNotExists_Success()
        {
            var filepath = IOHelper.CreateFileIfNotExists(typeof(TestEntity).Name, false);

            Assert.IsNotEmpty(filepath);
            Assert.IsTrue(File.Exists(filepath));
        }

        [Test]
        public void IO_CreateFileIfNotExists_Success2()
        {
            var filePath = IOHelper.CreateFileIfNotExists(typeof(TestEntity).Name, true);

            Assert.IsNotEmpty(filePath);
            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(File.Exists($"{filePath}.backup"));
        }

        [Test]
        public void IO_CreateFileIfNotExists_Fail()
        {
            Assert.Throws<IOException>(() => IOHelper.CreateFileIfNotExists("", true));
        }

        [Test]
        public void IO_RestoreFromBackUp_Success()
        {
            var filePath = IOHelper.CreateFileIfNotExists(typeof(TestEntity).Name, true);

            IOHelper.RestoreFromBackUp(new string[] { filePath });

            Assert.IsTrue(!File.Exists(filePath + ".backup"));
            Assert.IsTrue(File.Exists(filePath + ".restored"));
        }
    }
}
