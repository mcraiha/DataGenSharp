using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class IPv4GeneratorTests
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

			IPv4Generator ig1 = new IPv4Generator();

			// Act
			var shouldBeValidResult1 = ig1.Init(valid1, seed);

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

			IPv4Generator ig = new IPv4Generator();

			List<string> stringAddresses = new List<string>();
			List<uint> uintAddresses = new List<uint>();
			List<bool> successList = new List<bool>();

			// Act
			var shouldBeValidResult = ig.Init(parameter, seed);
			var generated1 = ig.Generate(parameter);
			var generated2 = ig.Generate(parameter); // Same step should provide same result
			
			for (int i = 0; i < loopCount; i++)
			{
				ig.NextStep();

				var generateResult1 = ig.Generate(parameter: null, wantedOutput: typeof(string));
				var generateResult2 = ig.Generate(parameter: null, wantedOutput: typeof(uint));

				successList.Add(generateResult1.success);
				successList.Add(generateResult2.success);
				
				stringAddresses.Add((string)generateResult1.result);
				uintAddresses.Add((uint)generateResult2.result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success, "Init should have been successful");

			Assert.AreEqual(generated1.result, generated2.result, "Both generates should have same results since NextStep has not been called between them");

			CollectionAssert.DoesNotContain(successList, false, "All generates should have been successful");
			CollectionAssert.AllItemsAreUnique(stringAddresses, "In generated addresses should be unique");
			CollectionAssert.AllItemsAreUnique(uintAddresses, "In generated addresses should be unique");

			Assert.AreEqual("67.94.2.170", stringAddresses[0], "Selected seed should always generate this result");
			Assert.AreEqual(2852281923, uintAddresses[0], "Selected seed should always generate this result");
		}
	}
}