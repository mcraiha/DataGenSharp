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

		[Test]
		public void TwoGeneratorTest()
		{
			// Arrange
			string expectedOutput = string.Join(Environment.NewLine, new string[]{
				"Id,Firstname,Lastname",
				"0,Jacob,Smith",
				"1,Sophia,Johnson",
				"2,Mason,Williams",
				"3,Isabella,Brown",
				"4,William,Jones",
				"5,Emma,Garcia",
				"6,Jayden,Miller",
				"7,Olivia,Davis",
				"8,Noah,Rodriguez",
				"9,Ava,Martinez",
				"" // Newline
			});

			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			GenerateData.chain.DataGenerators.Add(runningNumberGenerator);

			NameGenerator nameGenerator = new NameGenerator();
			nameGenerator.Init(null, seed: 1337);
			GenerateData.chain.DataGenerators.Add(nameGenerator);

			GenerateData.chain.OrderDefinition.Add(("Id", runningNumberGenerator, typeof(int), null, null));
			GenerateData.chain.OrderDefinition.Add(("Firstname", nameGenerator, typeof(string), null, "firstname"));
			GenerateData.chain.OrderDefinition.Add(("Lastname", nameGenerator, typeof(string), null, "lastname"));

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