using NUnit.Framework;
using DatagenSharp;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
	public class BooleanMutatorTests
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

			string valid1 = "cat,dog";
			string[] valid2 = new string[] {"cat", "dog"};
			int[] valid3 = new int[] {13, 24};

			object invalid1 = null;
			object invalid2 = new object();
			string invalid3 = "no commas";

			BooleanMutator bm1 = new BooleanMutator();
			BooleanMutator bm2 = new BooleanMutator();
			BooleanMutator bm3 = new BooleanMutator();
			BooleanMutator bm4 = new BooleanMutator();
			BooleanMutator bm5 = new BooleanMutator();
			BooleanMutator bm6 = new BooleanMutator();

			// Act
			var shouldBeValidResult1 = bm1.Init(valid1, seed);
			var shouldBeValidResult2 = bm2.Init(valid2, seed);
			var shouldBeValidResult3 = bm3.Init(valid3, seed);

			var shouldBeInvalidResult1 = bm4.Init(invalid1, seed);
			var shouldBeInvalidResult2 = bm5.Init(invalid2, seed);
			var shouldBeInvalidResult3 = bm6.Init(invalid3, seed);

			// Assert
			Assert.IsTrue(shouldBeValidResult1.success);
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidResult1.possibleError));

			Assert.IsTrue(shouldBeValidResult2.success);
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidResult2.possibleError));

			Assert.IsTrue(shouldBeValidResult3.success);
			Assert.IsTrue(string.IsNullOrEmpty(shouldBeValidResult3.possibleError));

			Assert.IsFalse(shouldBeInvalidResult1.success);
			Assert.IsFalse(string.IsNullOrEmpty(shouldBeInvalidResult1.possibleError));

			Assert.IsFalse(shouldBeInvalidResult2.success);
			Assert.IsFalse(string.IsNullOrEmpty(shouldBeInvalidResult2.possibleError));

			Assert.IsFalse(shouldBeInvalidResult3.success);
			Assert.IsFalse(string.IsNullOrEmpty(shouldBeInvalidResult3.possibleError));
		}

		[Test]
		public void ValidMutateTest()
		{
			// Arrange
			int seed = 1337;
			BooleanMutator valid1 = new BooleanMutator();
			BooleanMutator valid2 = new BooleanMutator();
			
			string sample1 = "cat,dog";
			int[] sample2 = new int[] {13, 24};

			bool[] inputArray = new bool[] { true, true, false, true, false};

			string[] expectedResults1 = new string[] {"cat", "cat", "dog", "cat", "dog"};
			int[] expectedResults2 = new int[] {13, 13, 24, 13, 24};
			string[] expectedResults3 = new string[] {"13", "13", "24", "13", "24"};

			List<bool> successArray1 = new List<bool>();
			List<bool> successArray2 = new List<bool>();
			List<bool> successArray3 = new List<bool>();

			List<string> resultArray1 = new List<string>();
			List<int> resultArray2 = new List<int>();
			List<string> resultArray3 = new List<string>();

			// Act
			var initResult1 = valid1.Init(sample1, seed);
			var initResult2 = valid2.Init(sample2, seed);

			foreach (bool b in inputArray)
			{
				(bool success1, string possibleError1, object result1) = valid1.Mutate(b);
				successArray1.Add(success1);
				resultArray1.Add((string)result1);

				(bool success2, string possibleError2, object result2) = valid2.Mutate(b);
				successArray2.Add(success2);
				resultArray2.Add((int)result2);

				(bool success3, string possibleError3, object result3) = valid2.Mutate(b, null, typeof(string));
				successArray3.Add(success3);
				resultArray3.Add((string)result3);
			}

			// Assert
			Assert.AreEqual(successArray1.Count, successArray1.Where(c => c).Count(), "Every run should be success");
			Assert.AreEqual(successArray2.Count, successArray2.Where(c => c).Count(), "Every run should be success");
			Assert.AreEqual(successArray3.Count, successArray3.Where(c => c).Count(), "Every run should be success");

			CollectionAssert.AreEqual(expectedResults1, resultArray1, "See that everything went as expected");
			CollectionAssert.AreEqual(expectedResults2, resultArray2, "See that everything went as expected");
			CollectionAssert.AreEqual(expectedResults3, resultArray3, "See that everything went as expected");
		}
	}
}