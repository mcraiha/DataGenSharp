using NUnit.Framework;
using DatagenSharp;
using System.IO;

namespace Tests
{
    public class GenerateDataTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void SimpleSmokeTest()
        {
            // Arrange
			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			GenerateData.chain.DataGenerators.Add(runningNumberGenerator);

			GenerateData.chain.OrderDefinition.Add(("Id", runningNumberGenerator, typeof(int), null, null));

			SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
			GenerateData.output = outCSV;

			MemoryStream ms = new MemoryStream();

			// Act
			GenerateData.Generate(ms);

			// Assert
			Assert.Greater(ms.Length, 1);
        }
    }
}