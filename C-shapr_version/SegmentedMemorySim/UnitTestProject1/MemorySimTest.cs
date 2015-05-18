using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegmentedMemorySim;

namespace UnitTestProject1
{
    [TestClass]
    public class MemorySimTest
    {
        [TestMethod]
        public void TestExample1()
        {
            RunTestExample(1);
        }

        [Ignore]
        public void TestExample2()
        {
            RunTestExample(2);
        }

        [Ignore]
        public void TestExample3()
        {
            RunTestExample(3);
        }

        [Ignore]
        public void TestExample4()
        {
            RunTestExample(4);
        }

        [Ignore]
        public void TestExample5()
        {
            RunTestExample(5);
        }

        [TestMethod]
        public void TestExample6()
        {
            RunTestExample(6);
        }

        private static void RunTestExample(int testNum)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var currentDir = Environment.CurrentDirectory;
                var inputDir = currentDir + @"\testData\test" + testNum + "_IN.txt";
                var outputDir = currentDir + @"\testData\test" + testNum + "_OUT.txt";

                var expected = File.ReadAllText(outputDir);

                var mm = new MemoryManager(new StreamReader(inputDir));
                mm.Run();

                Assert.AreEqual(expected, sw.ToString());
            }
        }
    }
}