using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class BitcoinAddressGeneratorTests
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

			object invalid1 = new object();

			int seed = 1337;

			BitcoinAddressGenerator bag1 = new BitcoinAddressGenerator();
			BitcoinAddressGenerator bag2 = new BitcoinAddressGenerator();

			// Act
			var shouldBeValidResult1 = bag1.Init(valid1, seed);

			var shouldBeInvalidResult1 = bag2.Init(invalid1, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success, "Init should have been successful");

			Assert.IsFalse(shouldBeInvalidResult1.success, "Init should have failed");
		}

		[Test]
		public void RandomGenerateTest()
		{
			// Arrange
			object parameter = null;

			int seed = 1337;

			int loopCount = 100;

			BitcoinAddressGenerator bag = new BitcoinAddressGenerator();

			List<string> btcAddresses = new List<string>();
			List<bool> successList = new List<bool>();

			// Act
			var shouldBeValidResult = bag.Init(parameter, seed);
			var generated1 = bag.Generate(parameter);
			var generated2 = bag.Generate(parameter); // Same step should provide same result
			
			for (int i = 0; i < loopCount; i++)
			{
				bag.NextStep();
				var generateResult = bag.Generate(parameter: null, wantedOutput: null);
				successList.Add(generateResult.success);
				btcAddresses.Add((string)generateResult.result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidResult.success, "Init should have been successful");

			Assert.AreEqual(generated1.result, generated2.result, "Both generates should have same results since NextStep has not been called between them");
			
			CollectionAssert.DoesNotContain(successList, false, "All generates should have been successful");
			CollectionAssert.AllItemsAreUnique(btcAddresses, "Generated addresses should be unique");

			Assert.AreEqual("17KwqxxCPJJHk3q1tws1Tsx7jTS7", (string)generated1.result, "With chosen seed first address should always be 17KwqxxCPJJHk3q1tws1Tsx7jTS7");
		}

		[Test, Description("Check that different seeds produce different outcome")]
		public void SeedTest()
		{
			// Arrange
			int seed1 = 1337;
			int seed2 = 13370;

			int rounds = 100;

			BitcoinAddressGenerator bag1 = new BitcoinAddressGenerator();
			BitcoinAddressGenerator bag2 = new BitcoinAddressGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = bag1.Init(null, seed1);
			var shouldBeValidInitResult2 = bag2.Init(null, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = bag1.Generate();
				var genResult2 = bag2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				bag1.NextStep();
				bag2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}

		[Test, Description("Check that save aka serialization generates correct text")]
		public void SaveTest()
		{
			// Arrange
			int seed = 1337;

			BitcoinAddressGenerator bag1 = new BitcoinAddressGenerator();

			// Act
			var shouldBeValidInitResult1 = bag1.Init(null, seed);
			string bg1String = bag1.Save();

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success);

			Assert.AreEqual($"~~{seed}", bg1String, "Default Init should store seed");
		}

		[Test, Description("Check that load aka deserialization can handle valid input")]
		public void LoadTest()
		{
			// Arrange
			int seed = 13237;

			string loadString = $"~~{seed}";

			BitcoinAddressGenerator bag1 = new BitcoinAddressGenerator();
			BitcoinAddressGenerator bag2 = new BitcoinAddressGenerator();
			BitcoinAddressGenerator bag3 = new BitcoinAddressGenerator();

			List<string> results1 = new List<string>();
			List<string> results2 = new List<string>();
			List<string> results3 = new List<string>();

			// Act
			var shouldBeValidInitResult1 = bag1.Init(null, seed);
			var shouldBeValidInitResult2 = bag3.Init(null, 0);

			var shouldBeValidLoadResult = bag2.Load(loadString);

			for (int i = 0; i < 13; i++)
			{
				results1.Add((string)bag1.Generate().result);
				results2.Add((string)bag2.Generate().result);
				results3.Add((string)bag3.Generate().result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success);
			Assert.IsTrue(shouldBeValidInitResult2.success);
			Assert.IsTrue(shouldBeValidLoadResult.success);

			CollectionAssert.AreEqual(results1, results2);
			CollectionAssert.AreNotEqual(results1, results3);
		}
	}
}