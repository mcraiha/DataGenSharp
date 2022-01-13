using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;

namespace Tests
{
	public class FileNameGeneratorTests
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

			FileNameGenerator fng1 = new FileNameGenerator();
			FileNameGenerator fng2 = new FileNameGenerator();

			// Act
			var shouldBeValidResult1 = fng1.Init(valid1, seed);

			var shouldBeInvalidResult1 = fng2.Init(invalid1, seed);

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

			FileNameGenerator fng = new FileNameGenerator();

			// Act
			var shouldBeValidResult = fng.Init(parameter, seed);
			var generated1 = fng.Generate(parameter);
			var generated2 = fng.Generate(parameter); // Same step should provide same result
			fng.NextStep(); // Step increase should alter the result for next generate
			var generated3 = fng.Generate(parameter);

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

			FileNameGenerator fng = new FileNameGenerator();

			// Act
			var shouldBeValidResult = fng.Init(initParameter, seed);
			var generated1 = fng.Generate();
			var generated2 = fng.Generate(); // Same step should provide same result
			fng.NextStep(); // Step increase should alter the result for next generate
			var generated3 = fng.Generate();

			for (int i = 0; i < 150; i++)
			{
				fng.NextStep();
				fng.Generate();
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success);

			Assert.IsTrue(generated1.success);
			Assert.IsTrue(generated2.success);
			Assert.IsTrue(generated3.success);

			Assert.AreEqual("MHT5077M.XSS", generated1.result, "MHT5077M.XSS should be the first filename in continous generation for file names");
			Assert.AreEqual("RuD0A251.Ac1", generated3.result, "RuD0A251.Ac1 should be the second filename in continous generation for file names");

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

			FileNameGenerator fng1 = new FileNameGenerator();
			FileNameGenerator fng2 = new FileNameGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = fng1.Init(parameter, seed1);
			var shouldBeValidInitResult2 = fng2.Init(parameter, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = fng1.Generate();
				var genResult2 = fng2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				fng1.NextStep();
				fng2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}
	}
}