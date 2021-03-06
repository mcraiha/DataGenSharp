using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DatagenSharp
{
	public partial class NameGenerator : IDataGenerator
	{
		public static readonly string LongName = "NameGenerator";

		public static readonly string ShortName = "NAME";
		
		public static readonly string Description = "Generator for outputting names, e.g. 'John Smith'";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedTypes = new Type[] { typeof(string) };

		private static readonly Dictionary<GenerateLanguage, (string firstNamesMaleResource, string firstNamesFemaleResource, string lastNamesResource)> languageResources = new Dictionary<GenerateLanguage, (string firstNamesMaleResource, string firstNamesFemaleResource, string lastNamesResource)>()
		{
			{ GenerateLanguage.EnglishUS,   ("Generators/Resources/EnglishUsMaleFirstNames.txt",    "Generators/Resources/EnglishUsFemaleFirstNames.txt",  "Generators/Resources/EnglishUsLastNames.txt")},
			{ GenerateLanguage.Finnish,     ("Generators/Resources/FinnishMaleFirstNames.txt",      "Generators/Resources/FinnishFemaleFirstNames.txt",    "Generators/Resources/FinnishLastNames.txt")},	
		};

		private static Dictionary<GenerateLanguage, (List<NameWeightPair> firstNamesMale, List<NameWeightPair> firstNamesFemale, List<NameWeightPair> lastNames)> languageDatas = new Dictionary<GenerateLanguage, (List<NameWeightPair> firstNamesMale, List<NameWeightPair> firstNamesFemale, List<NameWeightPair> lastNames)>();

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
			var chosenLanguageSettings = CheckAndGenerateLanguageDataIfNeeded(this.generateLanguage);
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

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		private void GenerateNumbers()
		{
			this.firstNameIndex = rng.Next(0, this.chosenFirstNames.Count);

			this.lastNameIndex = rng.Next(0, this.chosenLastNames.Count);
		}

		private void GenerateWeightedNumbers()
		{

		}

		private static (List<NameWeightPair> firstNamesMale, List<NameWeightPair> firstNamesFemale, List<NameWeightPair> lastNames) CheckAndGenerateLanguageDataIfNeeded(GenerateLanguage chosenLanguage)
		{
			// Check chosen language lists are already generated
			if (!languageDatas.ContainsKey(chosenLanguage))
			{
				// Generate those since they are missing
				var resourceNames = languageResources[chosenLanguage];
				var tempMaleFirstNames = LoadNames(resourceNames.firstNamesMaleResource);
				var tempFemaleFirstNames = LoadNames(resourceNames.firstNamesFemaleResource);
				var tempLastNames = LoadNames(resourceNames.lastNamesResource);

				languageDatas[chosenLanguage] = (tempMaleFirstNames, tempFemaleFirstNames, tempLastNames);
			}

			return languageDatas[chosenLanguage];
		}

		private static List<NameWeightPair> LoadNames(string resourceToLoad)
		{
			List<NameWeightPair> pairsToReturn = new List<NameWeightPair>();
			Stream namesStream = ResourceHelper.LoadResourceStream(resourceToLoad, typeof(NameGenerator).GetTypeInfo().Assembly);
			using (StreamReader sr = new StreamReader(namesStream)) 
            {
                string line;
                while ((line = sr.ReadLine()) != null) 
                {
					if (line.StartsWith("//"))
					{
						continue;
					}

                    string[] splitted = line.Split(',');
					pairsToReturn.Add(new NameWeightPair(splitted[0], int.Parse(splitted[1].Replace("_", ""))));
                }
            }

			return pairsToReturn;
		}
	}
}