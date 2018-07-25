using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class BooleanGenerator : IDataGenerator
	{
		public static readonly string LongName = "BooleanGenerator";
		
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
	}
}