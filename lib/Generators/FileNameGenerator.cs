using System;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public partial class FileNameGenerator : IDataGenerator
	{
		public static readonly string LongName = "FileNameGenerator";

		public static readonly string ShortName = "FILENAME";
		
		public static readonly string Description = "Generator for outputting file names";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedTypes = new Type[] { typeof(string) };

		private enum GenerateMode
		{
			Random = 0,
		};

        private enum WantedFileNameFormat
        {
            DOSCompatible_8_3 = 0,
        }

		private static readonly List<string> randomAliases = new List<string>() { "random", "rng" };

        private static readonly string randomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		private GenerateMode currentMode = GenerateMode.Random;

        private WantedFileNameFormat wantedFileNameFormat = WantedFileNameFormat.DOSCompatible_8_3;

        private string currentFilename = "";

        private Random rng = null;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{
				this.currentMode = GenerateMode.Random;
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
			if (this.currentMode == GenerateMode.Random)
			{
				this.rng = new Random(seed);
				this.GenerateFilename();
			}
			
			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			return (success: true, possibleError: "", result: this.currentFilename);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedTypes;
		}

		public void NextStep()
		{
			if (this.currentMode == GenerateMode.Random)
			{
				this.GenerateFilename();
			}	
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		private void GenerateFilename()
		{
            if (wantedFileNameFormat == WantedFileNameFormat.DOSCompatible_8_3)
            {
                string name = new string(Enumerable.Repeat(randomChars, 8).Select(s => s[this.rng.Next(s.Length)]).ToArray());
                string extension = new string(Enumerable.Repeat(randomChars, 3).Select(s => s[this.rng.Next(s.Length)]).ToArray());
                this.currentFilename = $"{name}.{extension}";
            }
		}
	}
}