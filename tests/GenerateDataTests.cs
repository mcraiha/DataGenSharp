using NUnit.Framework;
using DatagenSharp;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Tests
{
	public class GenerateDataTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void SimpleSmokeTest()
		{
			// Arrange
			string expectedOutput = string.Join(Environment.NewLine, new string[]{ 
				"Id",
				"0",
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"" // Newline
			});
			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			GenerateData.chain.DataGenerators.Add(runningNumberGenerator);

			GenerateData.chain.OrderDefinition.Add(("Id", runningNumberGenerator, typeof(int), null, null));

			SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
			GenerateData.output = outCSV;

			MemoryStream ms = new MemoryStream();

			// Act
			GenerateData.Generate(ms);
			string result = Encoding.UTF8.GetString(ms.ToArray());

			// Assert
			Assert.AreEqual(expectedOutput, result);
		}
	}
}