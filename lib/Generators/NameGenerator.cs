using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public partial class NameGenerator : IDataGenerator
	{
		public static readonly string LongName = "NameGenerator";
		
		public static readonly string Description = "Generator for outputting names, e.g. 'John Smith'";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedTypes = new Type[] { typeof(string) };

		private static readonly List<(GenerateLanguage language, List<NameWeightPair> firstNamesMale, List<NameWeightPair> firstNamesFemale, List<NameWeightPair> lastNames)> languagesAndNames = new List<(GenerateLanguage language, List<NameWeightPair> firstNamesMale, List<NameWeightPair> firstNamesFemale, List<NameWeightPair> lastNames)>()
		{
			(GenerateLanguage.EnglishUS,   EnglishUSMaleFirstNames,    EnglishUSFemaleFirstNames,  EnglishUSLastNames),
			(GenerateLanguage.Finnish,     FinnishMaleFirstNames,      FinnishFemaleFirstNames,    FinnishLastNames),		
		};

		private enum GenerateLanguage
		{
			EnglishUS = 0,
			Finnish = 1,
		}

		private static readonly List<string> randomAliases = new List<string>() {"random", "rng"};
		private static readonly List<string> weightedRandomAliases = new List<string>() {"weightedrandom", "wrng"};

		private static readonly List<string> languageAliases = new List<string>() {"language", "lang"};

		private enum GenerateMode
		{
			Continuos = 0,
			Random = 1,
			WeightedRandom = 2,
		};

		private GenerateLanguage generateLanguage = GenerateLanguage.EnglishUS;

		private GenerateMode currentMode = GenerateMode.Continuos;

		private int currentValue = 0;

		private int countedTotalWeight = -1;
		
		private List<NameWeightPair> chosenFirstNames = null;

		private List<NameWeightPair> chosenLastNames = null;

		private int firstNameIndex = -1;

		private int lastNameIndex = -1;

		private Random rng;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			// Check parameters (there might not be any)
			if (parameter == null)
			{
				this.currentMode = GenerateMode.Continuos;
				this.generateLanguage = GenerateLanguage.EnglishUS;
			}
			else if (parameter.GetType() == typeof(string))
			{
				var parsedParams = ParameterParser.GetParameterDictionary((string)parameter);
				
				if (ParameterParser.ContainsKey(parsedParams, randomAliases))
				{
					this.currentMode = GenerateMode.Random;
				}
				else if (ParameterParser.ContainsKey(parsedParams, weightedRandomAliases))
				{
					this.currentMode = GenerateMode.WeightedRandom;
				}
				else if (ParameterParser.ContainsKey(parsedParams, languageAliases))
				{
					foreach (string languageAlias in languageAliases)
					{
						if (parsedParams.ContainsKey(languageAlias))
						{
							foreach (string language in Enum.GetNames(typeof(GenerateLanguage)))
							{
								if (String.Equals(language, (string)parsedParams[languageAlias], StringComparison.InvariantCultureIgnoreCase))
								{
									this.generateLanguage = (GenerateLanguage)Enum.Parse(typeof(GenerateLanguage), language);
								}
							}
						}
					}
				}
			}
			else
			{
				string error = ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType());
				return (success: false, possibleError: error);
			}

			// Select language
			var chosenLanguageSettings = languagesAndNames.Find(element => element.language == this.generateLanguage);
			this.chosenFirstNames = NameMixer.CombineNames(chosenLanguageSettings.firstNamesMale, chosenLanguageSettings.firstNamesFemale);
			this.chosenLastNames = chosenLanguageSettings.lastNames;

			// Set values based on chosen GenerateMode
			if (this.currentMode == GenerateMode.Continuos)
			{
				this.currentValue = 0;
				this.firstNameIndex = 0;
				this.lastNameIndex = 0;
			}
			else if (this.currentMode == GenerateMode.Random)
			{
				this.rng = new Random(seed);
				this.GenerateNumbers();
			}
			else if (this.currentMode == GenerateMode.WeightedRandom)
			{
				this.rng = new Random(seed);
				this.GenerateWeightedNumbers();
			}

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			if (parameter == null)
			{
				// In case no parameters are give, return "Firstname Lastname"
				string name = $"{this.chosenFirstNames[this.firstNameIndex].name} {this.chosenLastNames[this.lastNameIndex].name}";
				return (success: true, possibleError: "", result: name);
			}

			if (parameter.GetType() == typeof(string))
			{
				string stringParameter = (string)parameter;
				stringParameter = stringParameter.ToLower();
				if (stringParameter == "firstname")
				{                
					string name = this.chosenFirstNames[this.firstNameIndex].name;
					return (success: true, possibleError: "", result: name);
				}
				else if (stringParameter == "lastname")
				{
					string name = this.chosenLastNames[this.lastNameIndex].name;
					return (success: true, possibleError: "", result: name);
				}
				else if (stringParameter == "fullname" || stringParameter == "fullnamefl")
				{
					string name = $"{this.chosenFirstNames[this.firstNameIndex].name} {this.chosenLastNames[this.lastNameIndex].name}";
					return (success: true, possibleError: "", result: name);
				}
				else if (stringParameter == "fullnamelf")
				{
					string name = $"{this.chosenLastNames[this.lastNameIndex].name} {this.chosenFirstNames[this.firstNameIndex].name}";
					return (success: true, possibleError: "", result: name);
				}
			}

			string error = ErrorMessages.UnsupportedParameterType(LongName, "Generate", parameter.GetType());
			return (success: false, possibleError: error, result: null);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedTypes;
		}

		public void NextStep()
		{
			if (this.currentMode == GenerateMode.Continuos)
			{
				this.currentValue++;
				this.firstNameIndex = this.currentValue % this.chosenFirstNames.Count;
				this.lastNameIndex = this.currentValue % this.chosenLastNames.Count;
			}
			else if (this.currentMode == GenerateMode.Random)
			{
				this.GenerateNumbers();
			}
			else if (this.currentMode == GenerateMode.WeightedRandom)
			{
				this.GenerateWeightedNumbers();
			}		
		}

		private void GenerateNumbers()
		{
			this.firstNameIndex = rng.Next(0, this.chosenFirstNames.Count);

			this.lastNameIndex = rng.Next(0, this.chosenLastNames.Count);
		}

		private void GenerateWeightedNumbers()
		{

		}
	}
}