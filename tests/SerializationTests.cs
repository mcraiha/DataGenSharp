using NUnit.Framework;
using DatagenSharp;
using System;

namespace Tests
{
	public class SerializationTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void SimpleGeneratorSerializationTest()
		{
			// Arrange
			GenerateData generateData = new GenerateData();
			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			generateData.AddGeneratorToChain(runningNumberGenerator);

			generateData.AddWantedElement(("Id", runningNumberGenerator, typeof(int), null, null));

			SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
			generateData.output = outCSV;

			string expected = $"DATAGEN VERSION 1{Environment.NewLine}DATA GENERATORS:{Environment.NewLine}1 RUN~~0{Environment.NewLine}MUTATOR CHAINS:{Environment.NewLine}WANTED ELEMENTS:{Environment.NewLine}-Id~1~_{Environment.NewLine}OUTPUTTER:{Environment.NewLine}XSV~{Environment.NewLine}";

			// Act
			string serialized = generateData.Save();

			// Assert
			Assert.AreEqual(expected, serialized);
		}
	}
}