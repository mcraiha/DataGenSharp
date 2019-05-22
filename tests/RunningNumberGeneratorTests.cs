using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class RunningNumberGeneratorTests
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

			RunningNumberGenerator rg1 = new RunningNumberGenerator();
			RunningNumberGenerator rg2 = new RunningNumberGenerator();


			// Act
			var shouldBeValidResult1 = rg1.Init(valid1, seed);

			var shouldBeInvalidResult1 = rg2.Init(invalid1, seed);

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

			RunningNumberGenerator rg1 = new RunningNumberGenerator();
			RunningNumberGenerator rg2 = new RunningNumberGenerator();
			RunningNumberGenerator rg3 = new RunningNumberGenerator();
			RunningNumberGenerator rg4 = new RunningNumberGenerator();
			RunningNumberGenerator rg5 = new RunningNumberGenerator();

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

			int start2 = 100;
			int step2 = 137;

			int start3 = -1;
			int step3 = -1;

			int start4 = 123456;
			int step4 = -1687;

			int start5 = int.MinValue;
			int step5 = 12405;

			// Act
			var shouldBeValidInitResult1 = rg1.Init(null, seed);
			var shouldBeValidInitResult2 = rg2.Init($"start={start2}|step={step2}", seed);
			var shouldBeValidInitResult3 = rg3.Init($"start={start3}|inc={step3}", seed);
			var shouldBeValidInitResult4 = rg4.Init($"begin={start4}|step={step4}", seed);
			var shouldBeValidInitResult5 = rg5.Init($"init={start5}|step={step5}", seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = rg1.Generate();
				var genResult2 = rg2.Generate();
				var genResult3 = rg3.Generate();
				var genResult4 = rg4.Generate();
				var genResult5 = rg5.Generate();

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

				rg1.NextStep();
				rg2.NextStep();
				rg3.NextStep();
				rg4.NextStep();
				rg5.NextStep();
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult1.possibleError), "Init should NOT have an error");

			Assert.IsTrue(shouldBeValidInitResult2.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult2.possibleError), "Init should NOT have an error");

			Assert.IsTrue(shouldBeValidInitResult3.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult3.possibleError), "Init should NOT have an error");

			Assert.IsTrue(shouldBeValidInitResult4.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult4.possibleError), "Init should NOT have an error");

			Assert.IsTrue(shouldBeValidInitResult5.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult5.possibleError), "Init should NOT have an error");

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

			CollectionAssert.AllItemsAreUnique(gene1Objects);
			Assert.AreEqual(0, gene1Objects[0]);
			Assert.AreEqual(99, gene1Objects[99]);

			CollectionAssert.AllItemsAreUnique(gene2Objects);
			Assert.AreEqual(start2, gene2Objects[0]);
			Assert.AreEqual(start2 + (rounds - 1) * step2, gene2Objects[99]);

			CollectionAssert.AllItemsAreUnique(gene3Objects);
			Assert.AreEqual(start3, gene3Objects[0]);
			Assert.AreEqual(start3 + (rounds - 1) * step3, gene3Objects[99]);

			CollectionAssert.AllItemsAreUnique(gene4Objects);
			Assert.AreEqual(start4, gene4Objects[0]);
			Assert.AreEqual(start4 + (rounds - 1) * step4, gene4Objects[99]);

			CollectionAssert.AllItemsAreUnique(gene5Objects);
			Assert.AreEqual(start5, gene5Objects[0]);
			Assert.AreEqual(start5 + (rounds - 1) * step5, gene5Objects[99]);
		}

		[Test, Description("Test that long generation works")]
		public void GenerateLongsTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			Type longType = typeof(long);

			RunningNumberGenerator rg1 = new RunningNumberGenerator();

			long intMax = int.MaxValue;

			List<object> gene1Objects = new List<object>(capacity: rounds);

			List<bool> gene1Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = rg1.Init($"start={intMax}|step=3", seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = rg1.Generate(null, longType);

				gene1Objects.Add(genResult1.result);

				gene1Success.Add(genResult1.success);

				rg1.NextStep();
			}

			// Assert
			Assert.AreEqual(rounds, gene1Objects.Count);

			Assert.AreEqual(rounds, gene1Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, longType, "There should be only longs generated");
			CollectionAssert.AllItemsAreUnique(gene1Objects);
		}

		[Test, Description("Check that floats are outputted")]
		public void FloatTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			Type floatType = typeof(float);

			RunningNumberGenerator rg1 = new RunningNumberGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<bool> gene1Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = rg1.Init(null, seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = rg1.Generate(null, wantedOutput: floatType);
				gene1Objects.Add(genResult1.result);
				gene1Success.Add(genResult1.success);

				rg1.NextStep();
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult1.possibleError), "Init should NOT have an error");

			Assert.AreEqual(rounds, gene1Objects.Count);
			Assert.AreEqual(rounds, gene1Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, floatType, "There should be only floats generated");
			CollectionAssert.AllItemsAreUnique(gene1Objects);
		}

		[Test, Description("Check that doubles are outputted")]
		public void DoubleTest()
		{
			// Arrange
			int seed = 1337;

			int rounds = 100;

			Type doubleType = typeof(double);

			RunningNumberGenerator rg1 = new RunningNumberGenerator();

			List<object> gene1Objects = new List<object>(capacity: rounds);
			List<bool> gene1Success = new List<bool>(capacity: rounds);

			// Act
			var shouldBeValidInitResult1 = rg1.Init(null, seed);

			for (int i = 0; i < rounds; i++)
			{
				var genResult1 = rg1.Generate(null, wantedOutput: doubleType);
				gene1Objects.Add(genResult1.result);
				gene1Success.Add(genResult1.success);

				rg1.NextStep();
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success, "Init should have been successful");
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidInitResult1.possibleError), "Init should NOT have an error");

			Assert.AreEqual(rounds, gene1Objects.Count);
			Assert.AreEqual(rounds, gene1Success.Count);

			CollectionAssert.AllItemsAreInstancesOfType(gene1Objects, doubleType, "There should be only doubles generated");
			CollectionAssert.AllItemsAreUnique(gene1Objects);
		}

		[Test, Description("Check that save aka serialization generates correct text")]
		public void SaveTest()
		{
			// Arrange
			int seed = 1337;

			RunningNumberGenerator rg1 = new RunningNumberGenerator();
			RunningNumberGenerator rg2 = new RunningNumberGenerator();
			RunningNumberGenerator rg3 = new RunningNumberGenerator();
			RunningNumberGenerator rg4 = new RunningNumberGenerator();

			int start2 = 100;
			int step2 = 137;
			string initString2 = $"start={start2}|step={step2}";

			// Act
			var shouldBeValidInitResult1 = rg1.Init(null, seed);
			string rg1String = rg1.Save();

			var shouldBeValidInitResult2 = rg2.Init(initString2, seed);
			string rg2String = rg2.Save();

			// Assert
			Assert.IsTrue(shouldBeValidInitResult1.success);
			Assert.IsTrue(shouldBeValidInitResult2.success);

			Assert.AreEqual("~~0", rg1String, "Default Init should not have anything to save");
			Assert.AreEqual($"~{initString2}~0", rg2String, "Init string should be saved");
		}

		[Test, Description("Check that load aka deserialization can handle valid input")]
		public void LoadTest()
		{
			// Arrange
			int seed = 1337;

			int start = 100;
			int step = 137;
			string initString = $"start={start}|step={step}";
			string loadString = $"~{initString}~0";

			RunningNumberGenerator rg1 = new RunningNumberGenerator();
			RunningNumberGenerator rg2 = new RunningNumberGenerator();

			List<int> results1 = new List<int>();
			List<int> results2 = new List<int>();

			// Act
			var shouldBeValidInitResult = rg1.Init(initString, seed);

			var shouldBeValidLoadResult = rg2.Load(loadString);

			for (int i = 0; i < 13; i++)
			{
				results1.Add((int)rg1.Generate().result);
				results2.Add((int)rg2.Generate().result);
			}

			// Assert
			Assert.IsTrue(shouldBeValidInitResult.success);
			Assert.IsTrue(shouldBeValidLoadResult.success);

			CollectionAssert.AreEqual(results1, results2);
		}
	}
}