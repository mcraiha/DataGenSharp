using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class IntegerGeneratorTests
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

			IntegerGenerator ig1 = new IntegerGenerator();
			IntegerGenerator ig2 = new IntegerGenerator();


			// Act
			var shouldBeValidResult1 = ig1.Init(valid1, seed);

			var shouldBeInvalidResult1 = ig2.Init(invalid1, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidResult1.possibleError), "Init should NOT have an error");

			Assert.IsFalse(shouldBeInvalidResult1.success, "Init should have failed");
			Assert.IsFalse(string.IsNullOrEmpty(shouldBeInvalidResult1.possibleError), "Init should have an error");
		}

		[Test, Description("Test that integer generation works")]
		public void GenerateIntsTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			IntegerGenerator ig1 = new IntegerGenerator();
			IntegerGenerator ig2 = new IntegerGenerator();
			IntegerGenerator ig3 = new IntegerGenerator();
			IntegerGenerator ig4 = new IntegerGenerator();
			IntegerGenerator ig5 = new IntegerGenerator();

			var range2 = (min: 2, max: 10);

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);
			List<object> gene3Objects = new List<object>(capacity: rounds);
			List<object> gene4Objects = new List<object>(capacity: rounds);
			List<object> gene5Objects = new List<object>(capacity: rounds);

			List<bool> gene1Success = new List<bool>(capacity: rounds);
			List<bool> gene2Success = new List<bool>(capacity: rounds);
			List<bool> gene3Success = new List<bool>(capacity: rounds);
			List<bool> gene4Success = new List<bool>(capacity: rounds);
			List<bool> gene5Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = ig1.Init(null, seed);
			var shouldBeValidInitResult2 = ig2.Init($"{range2.min}..{range2.max}", seed);
			var shouldBeValidInitResult3 = ig3.Init("-100,-66", seed);
			var shouldBeValidInitResult4 = ig4.Init($"{int.MinValue}..{int.MaxValue}", seed);
			var shouldBeValidInitResult5 = ig5.Init("0,1", seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = ig1.Generate();
				var genResult2 = ig2.Generate();
				var genResult3 = ig3.Generate();
				var genResult4 = ig4.Generate();
				var genResult5 = ig5.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);
				gene3Objects.Add(genResult3.result);
				gene4Objects.Add(genResult4.result);
				gene5Objects.Add(genResult5.result);

				gene1Success.Add(genResult1.success);
				gene2Success.Add(genResult2.success);
				gene3Success.Add(genResult3.success);
				gene4Success.Add(genResult4.success);
				gene5Success.Add(genResult5.success);

				ig1.NextStep();
				ig2.NextStep();
				ig3.NextStep();
				ig4.NextStep();
				ig5.NextStep();
			}

			// Assert
			Assert.AreEqual(rounds, gene1Objects.Count);
			Assert.AreEqual(rounds, gene2Objects.Count);
			Assert.AreEqual(rounds, gene3Objects.Count);
			Assert.AreEqual(rounds, gene4Objects.Count);
			Assert.AreEqual(rounds, gene5Objects.Count);

			Assert.AreEqual(rounds, gene1Success.Count);
			Assert.AreEqual(rounds, gene2Success.Count);
			Assert.AreEqual(rounds, gene3Success.Count);
			Assert.AreEqual(rounds, gene4Success.Count);
			Assert.AreEqual(rounds, gene5Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, typeof(int), "There should be only ints generated");
			CollectionAssert.AllItemsAreInstancesOfType(gene2Objects, typeof(int), "There should be only ints generated");
			CollectionAssert.AllItemsAreInstancesOfType(gene3Objects, typeof(int), "There should be only ints generated");
			CollectionAssert.AllItemsAreInstancesOfType(gene4Objects, typeof(int), "There should be only ints generated");
			CollectionAssert.AllItemsAreInstancesOfType(gene5Objects, typeof(int), "There should be only ints generated");

			var range1 = ig1.GetRange();
			foreach (object val in gene1Objects)
			{
				Assert.GreaterOrEqual((int)val, (int)range1.minRangeInclusive);
				Assert.Less((int)val, (int)range1.maxRangeExclusive);
			}

			Assert.AreEqual(range2.min, (int)ig2.GetRange().minRangeInclusive, "Range min should have been parsed correctly");
			Assert.AreEqual(range2.max, (int)ig2.GetRange().maxRangeExclusive, "Range max should have been parsed correctly");

			foreach (object val in gene2Objects)
			{
				Assert.GreaterOrEqual((int)val, range2.min);
				Assert.Less((int)val, range2.max);
			}
		}

		[Test, Description("Test that long generation works")]
		public void GenerateLongsTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			Type longType = typeof(long);

			IntegerGenerator ig1 = new IntegerGenerator();
			IntegerGenerator ig2 = new IntegerGenerator();
			IntegerGenerator ig3 = new IntegerGenerator();

			long intMin = int.MinValue;
			long intMax = int.MaxValue;
			var range1 = (min: intMin - 1, max: 10);
			var range2 = (min: -10, max: intMax + 1);
			var range3 = (min: long.MinValue, max: long.MaxValue);

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);
			List<object> gene3Objects = new List<object>(capacity: rounds);

			List<bool> gene1Success = new List<bool>(capacity: rounds);
			List<bool> gene2Success = new List<bool>(capacity: rounds);
			List<bool> gene3Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = ig1.Init($"{range1.min}..{range1.max}", seed);
			var shouldBeValidInitResult2 = ig2.Init($"{range2.min}..{range2.max}", seed);
			var shouldBeValidInitResult3 = ig3.Init($"{range3.min}..{range3.max}", seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = ig1.Generate(null, longType);
				var genResult2 = ig2.Generate(null, longType);
				var genResult3 = ig3.Generate(null, longType);

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);
				gene3Objects.Add(genResult3.result);

				gene1Success.Add(genResult1.success);
				gene2Success.Add(genResult2.success);
				gene3Success.Add(genResult3.success);

				ig1.NextStep();
				ig2.NextStep();
				ig3.NextStep();
			}

			// Assert
			Assert.AreEqual(rounds, gene1Objects.Count);
			Assert.AreEqual(rounds, gene2Objects.Count);
			Assert.AreEqual(rounds, gene3Objects.Count);

			Assert.AreEqual(rounds, gene1Success.Count);
			Assert.AreEqual(rounds, gene2Success.Count);
			Assert.AreEqual(rounds, gene3Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, longType, "There should be only longs generated");
			CollectionAssert.AllItemsAreInstancesOfType(gene2Objects, longType, "There should be only longs generated");
			CollectionAssert.AllItemsAreInstancesOfType(gene3Objects, longType, "There should be only longs generated");

			var rangeTemp = ig1.GetRange();
			foreach (object val in gene1Objects)
			{
				Assert.GreaterOrEqual((long)val, (long)rangeTemp.minRangeInclusive);
				Assert.Less((long)val, (long)rangeTemp.maxRangeExclusive);
			}

			Assert.AreEqual(range2.min, (long)ig2.GetRange().minRangeInclusive, "Range min should have been parsed correctly");
			Assert.AreEqual(range2.max, (long)ig2.GetRange().maxRangeExclusive, "Range max should have been parsed correctly");

			foreach (object val in gene2Objects)
			{
				Assert.GreaterOrEqual((long)val, range2.min);
				Assert.Less((long)val, range2.max);
			}
		}

		[Test, Description("Check that floats are outputted")]
		public void FloatTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			Type floatType = typeof(float);

			IntegerGenerator ig1 = new IntegerGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<bool> gene1Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = ig1.Init(null, seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = ig1.Generate(null, wantedOutput: floatType);
				gene1Objects.Add(genResult1.result);
				gene1Success.Add(genResult1.success);

				ig1.NextStep();
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult1.possibleError), "Init should NOT have an error");

			Assert.AreEqual(rounds, gene1Objects.Count);
			Assert.AreEqual(rounds, gene1Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, floatType, "There should be only floats generated");
		}

		[Test, Description("Check that doubles are outputted")]
		public void DoubleTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			Type doubleType = typeof(double);

			IntegerGenerator ig1 = new IntegerGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<bool> gene1Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = ig1.Init(null, seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = ig1.Generate(null, wantedOutput: doubleType);
				gene1Objects.Add(genResult1.result);
				gene1Success.Add(genResult1.success);

				ig1.NextStep();
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult1.possibleError), "Init should NOT have an error");

			Assert.AreEqual(rounds, gene1Objects.Count);
			Assert.AreEqual(rounds, gene1Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, doubleType, "There should be only doubles generated");
		}

		[Test, Description("Check that different seeds produce different outcome")]
		public void SeedTest()
		{
			// Arrange
			int seed1 = 1337;
			int seed2 = 13370;

			int rounds = 100;

			IntegerGenerator ig1 = new IntegerGenerator();
			IntegerGenerator ig2 = new IntegerGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<object> gene2Objects = new List<object>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = ig1.Init(null, seed1);
			var shouldBeValidInitResult2 = ig2.Init(null, seed2);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = ig1.Generate();
				var genResult2 = ig2.Generate();

				gene1Objects.Add(genResult1.result);
				gene2Objects.Add(genResult2.result);

				ig1.NextStep();
				ig2.NextStep();
			}

			CollectionAssert.AreNotEqual(gene1Objects, gene2Objects);
		}

		[Test, Description("Check that save aka serialization generates correct text")]
		public void SaveTest()
		{
			// Arrange
			int seed = 1337;

			IntegerGenerator ig1 = new IntegerGenerator();
			IntegerGenerator ig2 = new IntegerGenerator();

			string initString = $"1,33";

			// Act
			var shouldBeValidInitResult1 = ig1.Init(null, 0);
			string ig1String = ig1.Save();

			var shouldBeValidInitResult2 = ig2.Init(initString, seed);
			string ig2String = ig2.Save();

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success);
			Assert.IsTrue(shouldBeValidInitResult2.success);

			Assert.AreEqual("~~0", ig1String, "Default Init should not store any parameters");
			Assert.AreEqual($"~{initString}~{seed}", ig2String, "Init string should be saved");
		}

		[Test, Description("Check that load aka deserialization can handle valid input")]
		public void LoadTest()
		{
			// Arrange
			int seed = 13237;
			string range = "2,556";
			string loadString = $"~{range}~{seed}";

			IntegerGenerator ig1 = new IntegerGenerator();
			IntegerGenerator ig2 = new IntegerGenerator();
			IntegerGenerator ig3 = new IntegerGenerator();

			List<int> results1 = new List<int>();
			List<int> results2 = new List<int>();
			List<int> results3 = new List<int>();

			// Act
			var shouldBeValidInitResult1 = ig1.Init(range, seed);
			var shouldBeValidInitResult2 = ig3.Init(null, 0);

			var shouldBeValidLoadResult = ig2.Load(loadString);

			for (int i = 0; i < 13; i++)
			{
				results1.Add((int)ig1.Generate().result);
				results2.Add((int)ig2.Generate().result);
				results3.Add((int)ig3.Generate().result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success);
			Assert.IsTrue(shouldBeValidInitResult2.success);
			Assert.IsTrue(shouldBeValidLoadResult.success);

			CollectionAssert.AreEqual(results1, results2);
			Assert.IsTrue(results1.All(item => item >= 2 && item < 556));
			CollectionAssert.AreNotEqual(results1, results3);
		}
	}
}