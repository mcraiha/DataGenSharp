using NUnit.Framework;
using DatagenSharp;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

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

			GenerateData generateData = new GenerateData();
			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			generateData.AddGeneratorToChain(runningNumberGenerator);

			generateData.AddWantedElement(("Id", runningNumberGenerator, typeof(int), null, null));

			SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
			generateData.output = outCSV;

			MemoryStream ms = new MemoryStream();

			// Act
			generateData.Generate(ms);
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

			GenerateData generateData = new GenerateData();
			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			generateData.AddGeneratorToChain(runningNumberGenerator);

			NameGenerator nameGenerator = new NameGenerator();
			nameGenerator.Init(null, seed: 1337);
			generateData.AddGeneratorToChain(nameGenerator);

			generateData.AddWantedElement(("Id", runningNumberGenerator, null, typeof(int), null));
			generateData.AddWantedElement(("Firstname", nameGenerator, "firstname", typeof(string), null));
			generateData.AddWantedElement(("Lastname", nameGenerator, "lastname", typeof(string), null));

			SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
			generateData.output = outCSV;

			MemoryStream ms = new MemoryStream();

			// Act
			generateData.Generate(ms);
			string result = Encoding.UTF8.GetString(ms.ToArray());

			// Assert
			Assert.AreEqual(expectedOutput, result);
		}

		[Test]
		public void JsonOutputTest()
		{
			// Arrange
			GenerateData generateData = new GenerateData();
			RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
			generateData.AddGeneratorToChain(runningNumberGenerator);

			NameGenerator nameGenerator = new NameGenerator();
			nameGenerator.Init(null, seed: 1337);
			generateData.AddGeneratorToChain(nameGenerator);

			generateData.AddWantedElement(("Id", runningNumberGenerator, null, typeof(int), null));
			generateData.AddWantedElement(("Firstname", nameGenerator, "firstname", typeof(string), null));
			generateData.AddWantedElement(("Lastname", nameGenerator, "lastname", typeof(string), null));

			JsonOutput outJson = new JsonOutput();
			generateData.output = outJson;

			MemoryStream ms = new MemoryStream();

			// Act
			generateData.Generate(ms);
			string result = Encoding.UTF8.GetString(ms.ToArray());
			List<IdFirstnameLastname> idFirstnameLastnameList = JsonConvert.DeserializeObject<List<IdFirstnameLastname>>(result);
			
			// Assert
			Assert.AreEqual(generateData.howManyStepToRun, idFirstnameLastnameList.Count, "There should be certain amount of wanted items");
			Assert.AreEqual("Jacob", idFirstnameLastnameList[0].Firstname);
			Assert.AreEqual("Smith", idFirstnameLastnameList[0].Lastname);
			Assert.AreEqual(0, idFirstnameLastnameList[0].Id);
		}
	}

	public class IdFirstnameLastname
	{
		public int Id;
		public string Firstname;
		public string Lastname;
	}
}