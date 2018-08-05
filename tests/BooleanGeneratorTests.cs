using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class BooleanGeneratorTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void InitTest()
		{
			// Arrange
			object valid1 = null;
			string valid2 = "alter";
			string valid3 = "rng";

			object invalid1 = new object();

			int seed = 1337;

			BooleanGenerator bg1 = new BooleanGenerator();
			BooleanGenerator bg2 = new BooleanGenerator();
			BooleanGenerator bg3 = new BooleanGenerator();
			BooleanGenerator bg4 = new BooleanGenerator();

			// Act
			var shouldBeValidResult1 = bg1.Init(valid1, seed);
			var shouldBeValidResult2 = bg2.Init(valid2, seed);
			var shouldBeValidResult3 = bg3.Init(valid3, seed);

			var shouldBeInvalidResult1 = bg4.Init(invalid1, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success, "Init should have been successful");
			Assert.IsTrue(shouldBeValidResult2.success, "Init should have been successful");
			Assert.IsTrue(shouldBeValidResult3.success, "Init should have been successful");

			Assert.IsFalse(shouldBeInvalidResult1.success, "Init should have failed");
		}

		[Test]
		public void RandomGenerateTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			int loopCount = 100;

			BooleanGenerator bg = new BooleanGenerator();

			List<bool> bools = new List<bool>();
			List<bool> successList = new List<bool>();

			// Act
			var shouldBeValidResult = bg.Init(parameter, seed);
			var generated1 = bg.Generate(parameter);
			var generated2 = bg.Generate(parameter); // Same step should provide same result
			
			for (int i = 0; i < loopCount; i++)
			{
				bg.NextStep();
				var generateResult = bg.Generate(parameter: null, wantedOutput: null);
				successList.Add(generateResult.success);
				bools.Add((bool)generateResult.result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success, "Init should have been successful");

			Assert.AreEqual(generated1.result, generated2.result, "Both generates should have same results since NextStep has not been called between them");
			
			CollectionAssert.DoesNotContain(successList, false, "All generates should have been successful");
			Assert.GreaterOrEqual(bools.Where(item => item == true).Count(), 25, "There should be at least some true values");
			Assert.GreaterOrEqual(bools.Where(item => item == false).Count(), 25, "There should be at least some false values");
		}

		[Test]
		public void AlterGenerateTest()
		{
			// Arrange
			object parameter = "alter";

			int seed = 1337;

			int loopCount = 20;

			BooleanGenerator bg = new BooleanGenerator();

			List<bool> bools = new List<bool>();
			List<bool> successList = new List<bool>();

			// Act
			var shouldBeValidResult = bg.Init(parameter, seed);

			for (int i = 0; i < loopCount; i++)
			{
				var generateResult = bg.Generate(parameter: null, wantedOutput: null);
				successList.Add(generateResult.success);
				bools.Add((bool)generateResult.result);
				bg.NextStep();
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success, "Init should have been successful");
			CollectionAssert.DoesNotContain(successList, false, "All generates should have been successful");

			Assert.AreEqual(loopCount / 2, bools.Where(item => item == true).Count(), "Half of the results should be true");
			Assert.AreEqual(loopCount / 2, bools.Where(item => item == false).Count(), "Half of the results should be false");
		}

		[Test]
		public void WantedOutputTypeTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			BooleanGenerator bg = new BooleanGenerator();

			Type stringType = typeof(string);
			Type boolType = typeof(bool);
			Type doubleType = typeof(double);

			// Act
			var shouldBeValidInitResult = bg.Init(parameter, seed);

			var shouldBeValidGenerateResult1 = bg.Generate(parameter: null, wantedOutput: null);
			var shouldBeValidGenerateResult2 = bg.Generate(parameter: null, wantedOutput: stringType);
			var shouldBeValidGenerateResult3 = bg.Generate(parameter: null, wantedOutput: boolType);

			var shouldBeInvalidGenerateResult1 = bg.Generate(parameter: null, wantedOutput: doubleType);

			// Assert
			Assert.IsTrue(shouldBeValidGenerateResult1.success, "Generate should have succeeded");
			Assert.IsTrue(shouldBeValidGenerateResult2.success, "Generate should have succeeded");
			Assert.IsTrue(shouldBeValidGenerateResult3.success, "Generate should have succeeded");

			Assert.AreEqual(stringType, shouldBeValidGenerateResult2.result.GetType(), "Output should be string");
			Assert.AreEqual(true.ToString(), (string)shouldBeValidGenerateResult2.result, "Output should be 'True'");

			Assert.AreEqual(boolType, shouldBeValidGenerateResult3.result.GetType(), "Output should be bool");
			Assert.AreEqual(true, (bool)shouldBeValidGenerateResult3.result, "Output should be true");

			Assert.IsFalse(shouldBeInvalidGenerateResult1.success, "Generate should have failed for double");
			Assert.IsNull(shouldBeInvalidGenerateResult1.result, "Returned object should be null");
			Assert.IsFalse(string.IsNullOrEmpty(shouldBeInvalidGenerateResult1.possibleError), "There should be an error");
		}

		[Test, Description("Check that different seeds produce different outcome")]
		public void SeedTest()
		{
			// Arrange
			int seed1 = 1337;
			int seed2 = 13370;

			int rounds = 100;

			BooleanGenerator bg1 = new BooleanGenerator();
			BooleanGenerator bg2 = new BooleanGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = bg1.Init(null, seed1);
			var shouldBeValidInitResult2 = bg2.Init(null, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = bg1.Generate();
				var genResult2 = bg2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				bg1.NextStep();
				bg2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}
	}
}