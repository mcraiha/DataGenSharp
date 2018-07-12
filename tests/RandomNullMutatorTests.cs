using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class RandomNullMutatorTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void InitTest()
		{
			// Arrange
			int seed = 1337;
			RandomNullMutator valid1 = new RandomNullMutator();
			RandomNullMutator valid2 = new RandomNullMutator();
			RandomNullMutator valid3 = new RandomNullMutator();

			RandomNullMutator invalid1 = new RandomNullMutator();

			object sample1 = null;
			int sample2 = 30;
			double sample3 = 0.7;

			object sample4 = new object();

			// Act
			var initResult1 = valid1.Init(sample1, seed);
			var initResult2 = valid2.Init(sample2, seed);
			var initResult3 = valid3.Init(sample3, seed);

			var initResult4 = invalid1.Init(sample4, seed);

			// Assert
			Assert.IsTrue(initResult1.success);
			Assert.IsTrue(initResult2.success);
			Assert.IsTrue(initResult3.success);
			Assert.IsTrue(string.IsNullOrEmpty(initResult1.possibleError));
			Assert.IsTrue(string.IsNullOrEmpty(initResult2.possibleError));
			Assert.IsTrue(string.IsNullOrEmpty(initResult3.possibleError));

			Assert.IsFalse(initResult4.success);
			Assert.IsFalse(string.IsNullOrEmpty(initResult4.possibleError));
		}

		[Test]
		public void MutateTest()
		{
			// Arrange
			int seed = 1337;
			RandomNullMutator valid1 = new RandomNullMutator();
			object sample1 = null;
			var initResult1 = valid1.Init(sample1, seed);

			IEnumerable<int> numbers = Enumerable.Range(1, 100);
			string[] inputArray = numbers.Select(i => i.ToString()).ToArray();
			List<bool> successArray = new List<bool>();
			List<string> resultArray = new List<string>();

			// Act
			foreach (string str in inputArray)
			{
				(bool success, string possibleError, object result) = valid1.Mutate(str);
				successArray.Add(success);
				resultArray.Add((string)result);
			}

			// Assert
			CollectionAssert.AllItemsAreUnique(inputArray);
			CollectionAssert.DoesNotContain(inputArray, "null");

			CollectionAssert.AreNotEqual(inputArray, resultArray);
			CollectionAssert.Contains(resultArray, "null");
		}
	}
}