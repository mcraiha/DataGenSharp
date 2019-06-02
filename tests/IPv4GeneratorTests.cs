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

		[Test, Description("Check that save aka serialization generates correct text")]
		public void SaveTest()
		{
			// Arrange
			int seed = 1337;

			IPv4Generator ip1 = new IPv4Generator();

			// Act
			var shouldBeValidInitResult1 = ip1.Init(null, seed);
			string rg1String = ip1.Save();

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success);

			Assert.AreEqual($"~~{seed}", rg1String, "Default Init should have seed in save");
		}

		[Test, Description("Check that load aka deserialization can handle valid input")]
		public void LoadTest()
		{
			// Arrange
			int seed = 13367;

			string loadString = $"~~{seed}";

			IPv4Generator ip1 = new IPv4Generator();
			IPv4Generator ip2 = new IPv4Generator();

			List<string> results1 = new List<string>();
			List<string> results2 = new List<string>();

			// Act
			var shouldBeValidInitResult = ip1.Init(null, seed);

			var shouldBeValidLoadResult = ip2.Load(loadString);

			for (int i = 0; i < 13; i++)
			{
				results1.Add((string)ip1.Generate().result);
				results2.Add((string)ip2.Generate().result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult.success);
			Assert.IsTrue(shouldBeValidLoadResult.success);

			CollectionAssert.AreEqual(results1, results2);
		}
	}
}