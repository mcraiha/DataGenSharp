using NUnit.Framework;
using DatagenSharp;
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
		}
	}
}
