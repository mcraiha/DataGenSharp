using NUnit.Framework;
using DatagenSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
	public class SomeSeparatedValueOutputTests
	{
		// New line in current system
		private static string newLine = Environment.NewLine;

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void CsvSmokeTest()
		{
			// Arrange
			SomeSeparatedValueOutput csvOutput = new SomeSeparatedValueOutput();
			MemoryStream ms = new MemoryStream();
			string parameter = "csv";
			List<object> headers = new List<object> {"one", "two", "three"};
			List<object> values = new List<object> {1, 2, 3};
			string expectedOutput = $"one,two,three{newLine}1,2,3{newLine}";

			// Act
			(bool initSuccess, string possibleInitError) = csvOutput.Init(parameter, ms);
			(bool writeHeaderSuccess, string possibleWriteHeaderError) = csvOutput.WriteHeader(headers);
			(bool writeSingleSuccess, string possibleWriteSingleError) = csvOutput.WriteSingleEntry(values);
			(bool writeFooterSuccess, string possibleWriteFooterError) = csvOutput.WriteFooter(null);

			string result = Encoding.UTF8.GetString(ms.ToArray());

			// Assert
			Assert.IsTrue(initSuccess, "Init should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleInitError), "There should not be init error");

			Assert.IsTrue(writeHeaderSuccess, "Header write should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleWriteHeaderError), "There should not be write header error");

			Assert.IsTrue(writeSingleSuccess, "Single entry write should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleWriteSingleError), "There should not be single entry write error");

			Assert.IsTrue(writeFooterSuccess, "Footer write should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleWriteFooterError), "There should not be write footer error");

			Assert.AreEqual(expectedOutput, result);
		}

		[Test]
		public void TsvSmokeTest()
		{
			// Arrange
			SomeSeparatedValueOutput tsvOutput = new SomeSeparatedValueOutput();
			MemoryStream ms = new MemoryStream();
			string parameter = "tsv";
			List<object> headers = new List<object> {"one", "two", "three"};
			List<object> values = new List<object> {1, 2, 3};
			string expectedOutput = $"one\ttwo\tthree{newLine}1\t2\t3{newLine}";

			// Act
			(bool initSuccess, string possibleInitError) = tsvOutput.Init(parameter, ms);
			(bool writeHeaderSuccess, string possibleWriteHeaderError) = tsvOutput.WriteHeader(headers);
			(bool writeSingleSuccess, string possibleWriteSingleError) = tsvOutput.WriteSingleEntry(values);
			(bool writeFooterSuccess, string possibleWriteFooterError) = tsvOutput.WriteFooter(null);

			string result = Encoding.UTF8.GetString(ms.ToArray());

			// Assert
			Assert.IsTrue(initSuccess, "Init should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleInitError), "There should not be init error");

			Assert.IsTrue(writeHeaderSuccess, "Header write should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleWriteHeaderError), "There should not be write header error");

			Assert.IsTrue(writeSingleSuccess, "Single entry write should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleWriteSingleError), "There should not be single entry write error");

			Assert.IsTrue(writeFooterSuccess, "Footer write should have succeeded");
			Assert.IsTrue(string.IsNullOrEmpty(possibleWriteFooterError), "There should not be write footer error");

			Assert.AreEqual(expectedOutput, result);
		}
	}
}