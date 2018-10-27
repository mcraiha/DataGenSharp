using System;

namespace DatagenSharp
{
	public class RandomNullMutator : IMutator
	{
		public static readonly string LongName = "RandomNullMutator";

		public static readonly string ShortName = "RNGNULL";
		
		public static readonly string Description = "Replaces input value randomly with NULL";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedTypes = new Type[] { typeof(string) };

		private Random rng;

		private double threshold;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			bool thresholdParseSuccess = this.ParseThreshold(parameter);

			if (!thresholdParseSuccess)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			this.rng = new Random(seed);
			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Mutate(object input, object parameter = null, Type wantedOutputType = null)
		{
			bool thresholdParseSuccess = this.ParseThreshold(parameter);

			if (!thresholdParseSuccess)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Mutate", parameter.GetType()), result: null);
			}

			object returnValue = input;

			if (rng.NextDouble() < this.threshold)
			{
				returnValue = "null";
			}

			return (success: true, possibleError: "", result: returnValue);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedTypes;
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		private bool ParseThreshold(object parameter)
		{
			if (parameter == null)
			{
				this.threshold = 0.5;
				return true;
			}
			else if (parameter.GetType() == typeof(int))
			{
				this.threshold = (int)parameter / 100.0;
				return true;
			}
			else if (parameter.GetType() == typeof(double))
			{
				this.threshold = (double)parameter;
				return true;
			}

			return false;
		}
	}
}