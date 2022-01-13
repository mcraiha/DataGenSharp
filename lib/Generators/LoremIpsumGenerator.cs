using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public partial class LoremIpsumGenerator : IDataGenerator
	{
		public static readonly string LongName = "LoremIpsumGenerator";

		public static readonly string ShortName = "LOREMIPSUM";
		
		public static readonly string Description = "Generator for outputting Lorem Ipsum";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedTypes = new Type[] { typeof(string) };

		private static readonly List<string> words = new List<string>() { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod","tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };

		private enum GenerateMode
		{
			Continuos = 0,
			Random = 1,
		};

		private static readonly List<string> randomAliases = new List<string>() { "random", "rng" };

		private GenerateMode currentMode = GenerateMode.Continuos;

		private Random rng;

		private int wordIndex = -1;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{
				this.currentMode = GenerateMode.Continuos;
				this.wordIndex = 0;
			}
			else if (parameter.GetType() == typeof(string))
			{
				var parsedParams = ParameterParser.GetParameterDictionary((string)parameter);
				
				if (ParameterParser.ContainsKey(parsedParams, randomAliases))
				{
					this.currentMode = GenerateMode.Random;
				}
			}
			else
			{
				string error = ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType());
				return (success: false, possibleError: error);
			}

			// Set values based on chosen GenerateMode
			if (this.currentMode == GenerateMode.Continuos)
			{
				this.wordIndex = 0;
			}
			else if (this.currentMode == GenerateMode.Random)
			{
				this.rng = new Random(seed);
				this.GenerateNumbers();
			}
			
			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			string chosenWord = words[this.wordIndex];;
			return (success: true, possibleError: "", result: chosenWord);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedTypes;
		}

		public void NextStep()
		{
			if (this.currentMode == GenerateMode.Continuos)
			{
				this.wordIndex++;
				if (this.wordIndex >= words.Count)
				{
					this.wordIndex = 0;
				}
			}
			else if (this.currentMode == GenerateMode.Random)
			{
				this.GenerateNumbers();
			}	
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		private void GenerateNumbers()
		{
			this.wordIndex = rng.Next(0, words.Count);
		}
	}
}