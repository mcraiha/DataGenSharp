using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class PrivatizeMutatorTests
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
			PrivatizeMutator valid1 = new PrivatizeMutator();

			// Act
			var initResult1 = valid1.Init(null, seed);

			// Assert
			Assert.IsTrue(initResult1.success);

			Assert.IsTrue(string.IsNullOrEmpty(initResult1.possibleError));
		}

		[Test, Description("Basic privatize test")]
		public void ValidMutateTest()
		{
			// Arrange
			int seed = 1337;
			PrivatizeMutator valid1 = new PrivatizeMutator();
			string parameter = $"4{PrivatizeMutator.defaultValueSeparator}*";
			var initResult1 = valid1.Init(parameter, seed);

			string[] inputArray = new string[] { "123456789", "abcdefgh", "Very long text for this test case"};
			string[] expectedResults = new string[] {"1234*****", "abcd****", "Very*****************************"};

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
			CollectionAssert.AllItemsAreUnique(inputArray, "Make sure all inputs are unique");

			Assert.AreEqual(successArray.Count, successArray.Where(c => c).Count(), "Every run should be success");

			CollectionAssert.AreNotEqual(inputArray, resultArray, "Make sure Mutator modified the inputs");
			CollectionAssert.AreEqual(expectedResults, resultArray, "See that everything went as expected");
		}

        [Test, Description("Another privatize test")]
		public void ValidWithOtherParametersTest()
		{
			// Arrange
			int seed = 1337;
			PrivatizeMutator valid1 = new PrivatizeMutator();
			string parameter = $"0{PrivatizeMutator.defaultValueSeparator}X";
			var initResult1 = valid1.Init(parameter, seed);

			string[] inputArray = new string[] { "123456789", "abcdefgh", "Very long text for this test case"};
			string[] expectedResults = new string[] {"XXXXXXXXX", "XXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"};

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
			CollectionAssert.AllItemsAreUnique(inputArray, "Make sure all inputs are unique");

			Assert.AreEqual(successArray.Count, successArray.Where(c => c).Count(), "Every run should be success");

			CollectionAssert.AreNotEqual(inputArray, resultArray, "Make sure Mutator modified the inputs");
			CollectionAssert.AreEqual(expectedResults, resultArray, "See that everything went as expected");
		}
	}
}