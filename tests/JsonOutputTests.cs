using NUnit.Framework;
using DatagenSharp;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Tests
{
	public class JsonOutputTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void SmokeTest()
		{
			// Arrange
			var simpleTuple = (myInt: 1, myString: "text is nice");
			JsonOutput jsonOutput = new JsonOutput();
			object parameter = null;
			MemoryStream ms = new MemoryStream();
			List<object> headerEntries = new List<object>() { nameof(simpleTuple.myInt), nameof(simpleTuple.myString) };
			List<object> singleLineEntries = new List<object>() { simpleTuple.myInt, simpleTuple.myString };

			// Act
			var initResult = jsonOutput.Init(parameter, ms);
			var writeHeaderResult = jsonOutput.WriteHeader(headerEntries);
			var writeSingleEntryResult = jsonOutput.WriteSingleEntry(singleLineEntries);
			var writeFooterResult = jsonOutput.WriteFooter(null);

			string result = Encoding.UTF8.GetString(ms.ToArray());

			// Assert
			Assert.IsTrue(initResult.success, "Init should be successful");
			Assert.IsTrue(writeHeaderResult.success, "Header write should be successful");
			Assert.IsTrue(writeSingleEntryResult.success, "Writing single entry should be successful");
			Assert.IsTrue(writeFooterResult.success, "Writing footer should be successful");

			Assert.IsFalse(ms.CanWrite, "Footer write should have closed the stream");

			Assert.IsTrue(result.Contains((string)headerEntries[0]), "Final output should contain myInt");
			Assert.IsTrue(result.Contains((string)headerEntries[1]), "Final output should contain myString");

			Assert.IsTrue(result.Contains(singleLineEntries[0].ToString()), "Final output should contain 1");
			Assert.IsTrue(result.Contains((string)singleLineEntries[1]), "Final output should contain 'text is nice'");
		}
	}
}