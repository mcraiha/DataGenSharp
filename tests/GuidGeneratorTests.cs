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

			foreach (string guid in guids)
			{
				Assert.AreEqual(4, guid.Count(c => c== GuidGenerator.groupSeparator), "Every regular GUID should have 4 group separators");
			}
		}

		[Test, Description("Test that no grouping option works")]
		public void GenerateNoGroupingTest()
		{
			// Arrange
			string parameter = "JOINED"; // Disables grouping

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

			foreach (string guid in guids)
			{
				Assert.AreEqual(0, guid.Count(c => c== GuidGenerator.groupSeparator), "Every GUID should have 0 group separators");
			}
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

		[Test, Description("Check that different seeds produce different outcome")]
		public void SeedTest()
		{
			// Arrange
			int seed1 = 1337;
			int seed2 = 13370;

			int rounds = 100;

			GuidGenerator gg1 = new GuidGenerator();
			GuidGenerator gg2 = new GuidGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = gg1.Init(null, seed1);
			var shouldBeValidInitResult2 = gg2.Init(null, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = gg1.Generate();
				var genResult2 = gg2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				gg1.NextStep();
				gg2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}
	}
}