using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;

namespace Tests
{
	public class LoremIpsumGeneratorTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test, Description("Check that Init method works")]
		public void InitTest()
		{
			// Arrange
			object valid1 = null;

			object invalid1 = new object();

			int seed = 1337;

			LoremIpsumGenerator lig1 = new LoremIpsumGenerator();
			LoremIpsumGenerator lig2 = new LoremIpsumGenerator();

			// Act
			var shouldBeValidResult1 = lig1.Init(valid1, seed);

			var shouldBeInvalidResult1 = lig2.Init(invalid1, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success);

			Assert.IsFalse(shouldBeInvalidResult1.success);
		}

		[Test, Description("Check that Generate and NextStep methods work")]
		public void ContinuosGenerateTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			LoremIpsumGenerator lig = new LoremIpsumGenerator();

			// Act
			var shouldBeValidResult = lig.Init(parameter, seed);
			var generated1 = lig.Generate(parameter);
			var generated2 = lig.Generate(parameter); // Same step should provide same result
			lig.NextStep(); // Step increase should alter the result for next generate
			var generated3 = lig.Generate(parameter);

			// Assert
			Assert.IsTrue(shouldBeValidResult.success);

			Assert.IsTrue(generated1.success);
			Assert.IsTrue(generated2.success);
			Assert.IsTrue(generated3.success);

			Assert.AreEqual(generated1.result, generated2.result);
			Assert.AreNotEqual(generated1.result, generated3.result);
		}

		[Test, Description("Check generating long amount produces wanted output")]
		public void ContinuosGenerateLongAmountTest()
		{
			// Arrange
			object initParameter = null;

			int seed = 1337;

			LoremIpsumGenerator lig = new LoremIpsumGenerator();

			// Act
			var shouldBeValidResult = lig.Init(initParameter, seed);
			var generated1 = lig.Generate();
			var generated2 = lig.Generate(); // Same step should provide same result
			lig.NextStep(); // Step increase should alter the result for next generate
			var generated3 = lig.Generate();

			for (int i = 0; i < 150; i++)
			{
				lig.NextStep();
				lig.Generate();
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success);

			Assert.IsTrue(generated1.success);
			Assert.IsTrue(generated2.success);
			Assert.IsTrue(generated3.success);

			Assert.AreEqual("lorem", generated1.result, "lorem should be the first word in continous generation for lorem ipsum");
			Assert.AreEqual("ipsum", generated3.result, "ipsum should be the second word in continous generation for lorem ipsum");

			Assert.AreEqual(generated1.result, generated2.result);
			Assert.AreNotEqual(generated1.result, generated3.result);
		}

		[Test, Description("Check that different seeds produce different outcome")]
		public void SeedTest()
		{
			// Arrange
			int seed1 = 1337;
			int seed2 = 13370;

			int rounds = 100;

			string parameter = "random";

			LoremIpsumGenerator lig1 = new LoremIpsumGenerator();
			LoremIpsumGenerator lig2 = new LoremIpsumGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = lig1.Init(parameter, seed1);
			var shouldBeValidInitResult2 = lig2.Init(parameter, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = lig1.Generate();
				var genResult2 = lig2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				lig1.NextStep();
				lig2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}
	}
}