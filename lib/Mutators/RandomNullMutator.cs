using System;
using System.Text;

namespace DatagenSharp
{
	public sealed class RandomNullMutator : IMutator, ISerialization
	{
		public static readonly string LongName = "RandomNullMutator";

		public static readonly string ShortName = "RNGNULL";
		
		public static readonly string Description = "Replaces input value randomly with NULL";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedTypes = new Type[] { typeof(string) };

		private Random rng;

		private double threshold;

		/// <summary>
		/// Stored seed, needed only for serialization purposes because there is no easy way to get seed back from Random
		/// </summary>
		private int storedSeed = 0;

		private long internalId = 0;

		public RandomNullMutator()
		{
			this.internalId = UniqueIdMaker.GetId();
		}

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			bool thresholdParseSuccess = this.ParseThreshold(parameter);

			if (!thresholdParseSuccess)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			this.rng = new Random(seed);
			this.storedSeed = seed;
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

		#region Serialization
		public (bool success, string possibleError) Load(string parameter)
		{
			return (true, "");
		}

		public string Save()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{CommonSerialization.delimiter}{CommonSerialization.delimiter}{this.storedSeed}");

			return sb.ToString();
		}

		public long GetInternalId()
		{
			return this.internalId;
		}

		#endregion // Serialization
	}
}