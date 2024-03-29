using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;

namespace Tests
{
	public class NameGeneratorTests
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
			string valid2 = "finnish";

			object invalid1 = new object();

			int seed = 1337;

			NameGenerator ng1 = new NameGenerator();
			NameGenerator ng2 = new NameGenerator();
			NameGenerator ng3 = new NameGenerator();

			// Act
			var shouldBeValidResult1 = ng1.Init(valid1, seed);
			var shouldBeValidResult2 = ng2.Init(valid2, seed);

			var shouldBeInvalidResult1 = ng3.Init(invalid1, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success);
			Assert.IsTrue(shouldBeValidResult2.success);

			Assert.IsFalse(shouldBeInvalidResult1.success);
		}

		[Test, Description("Check that different languages provide different output")]
		public void CheckLanguageDifferencesTest()
		{
			// Arrange
			object valid1 = null; // Should default to English
			string valid2 = "language=finnish";

			int seed = 1337;

			NameGenerator ng1 = new NameGenerator();
			NameGenerator ng2 = new NameGenerator();

			// Act
			var shouldBeValidResult1 = ng1.Init(valid1, seed);
			var shouldBeValidResult2 = ng2.Init(valid2, seed);

			var generated1 = ng1.Generate();
			var generated2 = ng2.Generate();

			// Assert
			Assert.AreNotEqual((string)generated1.result, (string)generated2.result);
		}

		[Test]
		public void ContinuosGenerateTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			NameGenerator ng = new NameGenerator();

			// Act
			var shouldBeValidResult = ng.Init(parameter, seed);
			var generated1 = ng.Generate(parameter);
			var generated2 = ng.Generate(parameter); // Same step should provide same result
			ng.NextStep(); // Step increase should alter the result for next generate
			var generated3 = ng.Generate(parameter);

			// Assert
			Assert.IsTrue(shouldBeValidResult.success);

			Assert.IsTrue(generated1.success);
			Assert.IsTrue(generated2.success);
			Assert.IsTrue(generated3.success);

			Assert.AreEqual(generated1.result, generated2.result);
			Assert.AreNotEqual(generated1.result, generated3.result);
		}

		[Test]
		public void ContinuosGenerateFinnishLastNamesTest()
		{
			// Arrange
			object initParameter = "language=finnish";
			object generateParameter = "lastname";

			int seed = 1337;

			NameGenerator ng = new NameGenerator();

			// Act
			var shouldBeValidResult = ng.Init(initParameter, seed);
			var generated1 = ng.Generate(generateParameter);
			var generated2 = ng.Generate(generateParameter); // Same step should provide same result
			ng.NextStep(); // Step increase should alter the result for next generate
			var generated3 = ng.Generate(generateParameter);

			// Assert
			Assert.IsTrue(shouldBeValidResult.success);

			Assert.IsTrue(generated1.success);
			Assert.IsTrue(generated2.success);
			Assert.IsTrue(generated3.success);

			Assert.AreEqual("Korhonen", generated1.result, "Korhonen should be the first name in continous generation for finnish last names");
			Assert.AreEqual("Virtanen", generated3.result, "Virtanen should be the second name in continous generation for finnish last names");

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

			NameGenerator ng1 = new NameGenerator();
			NameGenerator ng2 = new NameGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = ng1.Init(parameter, seed1);
			var shouldBeValidInitResult2 = ng2.Init(parameter, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = ng1.Generate();
				var genResult2 = ng2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				ng1.NextStep();
				ng2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}
	}
}