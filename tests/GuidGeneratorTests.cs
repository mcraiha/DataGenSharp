using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class GuidGeneratorTests
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
			int seed = 1337;

			GuidGenerator gg1 = new GuidGenerator();

			// Act
			var shouldBeValidResult1 = gg1.Init(valid1, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidResult1.possibleError), "Init should NOT have an error");
		}

		[Test]
		public void GenerateTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			int loopCount = 100;

			GuidGenerator gg = new GuidGenerator();

			List<string> guids = new List<string>();
			List<bool> successList = new List<bool>();

			// Act
			var shouldBeValidResult = gg.Init(parameter, seed);
			var generated1 = gg.Generate(parameter);
			var generated2 = gg.Generate(parameter); // Same step should provide same result
			
			for (int i = 0; i < loopCount; i++)
			{
				gg.NextStep();
				var generateResult = gg.Generate(parameter: null, wantedOutput: null);
				successList.Add(generateResult.success);
				guids.Add((string)generateResult.result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success, "Init should have been successful");

			Assert.AreEqual(generated1.result, generated2.result, "Both generates should have same results since NextStep has not been called between them");
			
			CollectionAssert.DoesNotContain(successList, false, "All generates should have been successful");
			CollectionAssert.AllItemsAreUnique(guids, "In theory every GUID should be unique");
		}

		[Test]
		public void CheckThatCanBeParsedTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			int loopCount = 100;

			GuidGenerator gg = new GuidGenerator();

			List<bool> parseWasOk = new List<bool>();

			// Act
			var shouldBeValidResult = gg.Init(parameter, seed);
			for (int i = 0; i < loopCount; i++)
			{
				var generateResult = gg.Generate(parameter: null, wantedOutput: null);
				gg.NextStep();
				parseWasOk.Add(Guid.TryParse((string)generateResult.result, out _));
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success, "Init should have been successful");
			Assert.AreEqual(parseWasOk.Count, parseWasOk.Where(item => item == true).Count(), "All of the results should be true");
		}
	}
}