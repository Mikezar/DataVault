using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataVault.ConsoleTest
{
    class TestEntity
    {
        public int Test { get; set; }

    }


    class Program
    {

        static void Main(string[] args)
        {
            var prop = typeof(TestEntity).GetProperties();

            // Console.WriteLine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString());

            Console.ReadKey();
        }
    }
}
