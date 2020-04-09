using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class RunningNumberGenerator : IDataGenerator, ISerialization
	{
		public static readonly string LongName = "RunningNumberGenerator";

		public static readonly string ShortName = "RUN";

		public static readonly string Description = "Generate run of values (1, 2, 3 etc.)";

		public static readonly string VersionNumber = "0.91";

		private static readonly Type[] supportedTypes = new Type[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(string) };

		private static readonly List<string> startAliases = new List<string>() {"start", "init", "begin"};

		private static readonly List<string> stepAliases = new List<string>() {"step", "inc", "increase"};

		private static readonly long currentValueDefault = 0;
		private long currentValue = currentValueDefault;

		private static readonly long stepDefault = 1;
		private long step = stepDefault;

		private long internalId = 0;

		public RunningNumberGenerator()
		{
			this.internalId = UniqueIdMaker.GetId();
		}

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{
				// If no parameters are given, then go with defaults
			}
			else if (parameter.GetType() == typeof(string))
			{
				var parsedParams = ParameterParser.GetParameterDictionary((string)parameter);

				if (ParameterParser.ContainsKey(parsedParams, startAliases))
				{
					string potentialStartValue = ParameterParser.GetValueWithKeys(parsedParams, startAliases);
					if (!long.TryParse(potentialStartValue, out this.currentValue))
					{
						return (success: false, possibleError: $"{potentialStartValue} is not a number!");
					}
				}

				if (ParameterParser.ContainsKey(parsedParams, stepAliases))
				{
					string potentialStepValue = ParameterParser.GetValueWithKeys(parsedParams, stepAliases);
					if (!long.TryParse(potentialStepValue, out this.step))
					{
						return (success: false, possibleError: $"{potentialStepValue} is not a number!");
					}
				}
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			object returnValue = this.currentValue;

			if (wantedOutput == null || wantedOutput == typeof(int))
			{
				// Limit to int if needed
				if (this.currentValue < int.MinValue)
				{
					returnValue = int.MinValue;
				}
				else if (this.currentValue > int.MaxValue)
				{
					returnValue = int.MaxValue;
				}
				else
				{
					returnValue = (int)this.currentValue;
				}
			}
			else if (wantedOutput == typeof(long))
			{
				returnValue = this.currentValue;
			}
			else if (wantedOutput == typeof(float))
			{
				// Cast to float
				returnValue = (float)this.currentValue;
			}
			else if (wantedOutput == typeof(double))
			{
				// Cast to double
				returnValue = (double)this.currentValue;
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
			return supportedTypes;
		}

		public void NextStep()
		{
			this.currentValue += this.step;
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		/// <summary>
		/// Deserialize RunningNumberGenerator
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
			return this.Init(splitted[0], 0);
		}

		/// <summary>
		/// Serialize RunningNumberGenerator 
		/// </summary>
		/// <returns>String serialization</returns>
		public string Save()
		{
			// Only serialize current value if it isn't default
			string currentValueAsString = "";
			if (this.currentValue != currentValueDefault)
			{
				currentValueAsString = $"{startAliases[0]}{ParameterParser.valueSeparator}{this.currentValue}";
			}

			// Only serialize step value if it isn't default
			string stepAsString = "";
			if (this.step != stepDefault)
			{
				stepAsString = $"{stepAliases[0]}{ParameterParser.valueSeparator}{this.step}";
			}

			string joined = "";

			if (!string.IsNullOrEmpty(currentValueAsString) && !string.IsNullOrEmpty(stepAsString))
			{
				joined = $"{currentValueAsString}{ParameterParser.entrySeparator}{stepAsString}";
			}
			else if (!string.IsNullOrEmpty(currentValueAsString) && string.IsNullOrEmpty(stepAsString))
			{
				joined = currentValueAsString;
			}
			else if (string.IsNullOrEmpty(currentValueAsString) && !string.IsNullOrEmpty(stepAsString))
			{
				joined = stepAsString;
			}

			return $"{CommonSerialization.delimiter}{joined}{CommonSerialization.delimiter}0";
		}

		public long GetInternalId()
		{
			return this.internalId;
		}

	}
}