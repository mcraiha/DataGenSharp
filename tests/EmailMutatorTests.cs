using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class EmailMutatorTests
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
			EmailMutator valid1 = new EmailMutator();

			// Act
			var initResult1 = valid1.Init(null, seed);

			// Assert
			Assert.IsTrue(initResult1.success);

			Assert.IsTrue(string.IsNullOrEmpty(initResult1.possibleError));
		}

		[Test]
		public void ValidMutateTest()
		{
			// Arrange
			int seed = 1337;
			EmailMutator valid1 = new EmailMutator();
			object sample1 = null;
			var initResult1 = valid1.Init(sample1, seed);

			string[] inputArray = new string[] { "John Smith", "Pele", "Mick Han Jackson"};
			string[] expectedResults = new string[] {"John.Smith@hotmail.com", "Pele@yahoo.com", "Mick.Han.Jackson@msn.com"};

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