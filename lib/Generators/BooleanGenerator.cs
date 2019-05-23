using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class BooleanGenerator : IDataGenerator
	{
		public static readonly string LongName = "BooleanGenerator";
		public static readonly string ShortName = "BOOL";
		
		public static readonly string Description = "Generate boolean values (true or false)";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(bool), typeof(string) };

		private static readonly List<string> randomModeKeywords = new List<string>() {"random", "rng"};

		private static readonly List<string> alternatingModeKeywords = new List<string>() {"alter", "alternating"};

		private enum GenerateMode
		{
			Random = 0,
			Alternating
		};

		private GenerateMode chosenMode = GenerateMode.Random;
		private Random rng = null;

		private bool currentValue = true;

		/// <summary>
		/// Stored seed, needed only for serialization purposes because there is no easy way to get seed back from Random
		/// </summary>
		private int storedSeed = 0;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{

			}
			else if (parameter.GetType() == typeof(string))
			{
				string parameterAsString = (string)parameter;
				parameterAsString = parameterAsString.ToLower();
				if (randomModeKeywords.Contains(parameterAsString))
				{
					this.chosenMode = GenerateMode.Random;
				}
				else if (alternatingModeKeywords.Contains(parameterAsString))
				{
					this.chosenMode = GenerateMode.Alternating;
				}
				else
				{
					return (success: false, possibleError: ErrorMessages.UnsupportedParameterValue(LongName, "Init", parameterAsString));
				}
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			if (this.chosenMode == GenerateMode.Random)
			{
				this.storedSeed = seed;
				this.rng = new Random(seed);
				this.currentValue = rng.Next(0, 2) == 0;
			}
			else if (this.chosenMode == GenerateMode.Alternating)
			{
				// TODO: Process current value override here

			}

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			object returnValue = this.currentValue;

			if (wantedOutput == null || wantedOutput == typeof(bool))
			{

			}
			else if (wantedOutput == typeof(string))
			{
				returnValue = this.currentValue.ToString();
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutput), result: null);
			}

			return (success: true, possibleError: "", result: returnValue);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedOutputTypes;
		}

		public void NextStep()
		{
			if (this.chosenMode == GenerateMode.Random)
			{
				// Generate new
				this.currentValue = rng.Next(0, 2) == 0;
			}
			else if (this.chosenMode == GenerateMode.Alternating)
			{
				// Alternate between true and false
				this.currentValue = !this.currentValue;
			}
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		/// <summary>
		/// Deserialize BooleanGenerator
		/// </summary>
		/// <param name="parameter">String to deserialize</param>
		/// <returns>Tuple that tells if everything went well, and possible error message</returns>
		public (bool success, string possibleError) Load(string parameter)
		{
			if (!CommonSerialization.IsSomewhatValidGeneratorSaveData(parameter))
			{
				return (success: false, possibleError: $"Parameter: {parameter} given to {LongName} does NOT fulfill the requirements!");
			}

			string[] splitted = CommonSerialization.SplitGeneratorSaveData(parameter);
			return this.Init(splitted[0], int.Parse(splitted[1]));
		}

		/// <summary>
		/// Serialize BooleanGenerator
		/// </summary>
		/// <returns>String serialization</returns>
		public string Save()
		{
			string mode = this.chosenMode == GenerateMode.Random ? randomModeKeywords[0] : alternatingModeKeywords[0];
			int seed = this.chosenMode == GenerateMode.Random ? this.storedSeed : 0;
			return $"{CommonSerialization.delimiter}{mode}{CommonSerialization.delimiter}{seed}";
		}
	}
}