using NUnit.Framework;
using DatagenSharp;
using System;

namespace Tests
{
	public class DeserializationTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void SimpleGeneratorDeserializationTest()
		{
			// Arrange
			string input = $"DATAGEN VERSION 1{Environment.NewLine}DATA GENERATORS:{Environment.NewLine}1 RUN~~0{Environment.NewLine}MUTATOR CHAINS:{Environment.NewLine}WANTED ELEMENTS:{Environment.NewLine}-Id~1~_{Environment.NewLine}OUTPUTTER:{Environment.NewLine}XSV~{Environment.NewLine}";
			GenerateData generateData = new GenerateData();

			// Act
			(bool success, string possibleError) = generateData.Load(input);

			// Assert
			Assert.IsTrue(success);
			Assert.IsTrue(string.IsNullOrEmpty(possibleError));
			Assert.AreEqual(1, generateData.chain.DataGenerators.Count);
			Assert.AreEqual(1, generateData.chain.WantedElements.Count);
			Assert.IsNotNull(generateData.output);
		}
	}
}