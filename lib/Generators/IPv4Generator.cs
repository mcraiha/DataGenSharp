using System;
using System.Text;
using System.Collections.Generic;

namespace DatagenSharp
{
	public sealed class IPv4Generator : IDataGenerator, ISerialization
	{
		public static readonly string LongName = "IPv4Generator";

		public static readonly string ShortName = "IPV4";
		
		public static readonly string Description = "Generate IPv4 addresses (e.g. 145.11.23.1)";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string), typeof(uint) };

		private static readonly char dotSeparator = '.';

		private Random rng = null;

		private byte[] currentValue = new byte[4];

		/// <summary>
		/// Stored seed, needed only for serialization purposes because there is no easy way to get seed back from Random
		/// </summary>
		private int storedSeed = 0;

		private long internalId = 0;

		private enum SelectedOutput 
		{
			StringWithDotSeparators = 0,
			Uint
		}

		public IPv4Generator()
		{
			this.internalId = UniqueIdMaker.GetId();
		}

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			this.rng = new Random(seed);

			this.storedSeed = seed;

			// Generate first value
			this.NextStep();

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			SelectedOutput selectedOutput = SelectedOutput.StringWithDotSeparators;
			if (wantedOutput == null)
			{
				selectedOutput = SelectedOutput.StringWithDotSeparators;
			}
			else if (Array.IndexOf(supportedOutputTypes, wantedOutput) > -1)
			{
				if (wantedOutput == typeof(string))
				{
					selectedOutput = SelectedOutput.StringWithDotSeparators;
				}
				else if (wantedOutput == typeof(uint))
				{
					selectedOutput = SelectedOutput.Uint;
				}
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutput), result: null);
			}

			object returnValue = null;
			if (selectedOutput == SelectedOutput.StringWithDotSeparators)
			{
				returnValue = GetAsString(this.currentValue);
			}
			else if (selectedOutput == SelectedOutput.Uint)
			{
				returnValue = GetAsUint(this.currentValue);
			}

			return (success: true, possibleError: "", result: returnValue);
		}

		private static string GetAsString(byte[] bytes)
		{
			StringBuilder returnValue = new StringBuilder(16);

			returnValue.Append($"{bytes[0]}{dotSeparator}{bytes[1]}{dotSeparator}{bytes[2]}{dotSeparator}{bytes[3]}");

			return returnValue.ToString();
		}

		private static uint GetAsUint(byte[] bytes)
		{
			return BitConverter.ToUInt32(bytes, 0);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedOutputTypes;
		}

		public void NextStep()
		{
			this.rng.NextBytes(currentValue);
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		/// <summary>
		/// Deserialize IPv4Generator
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
		/// Serialize IPv4Generator
		/// </summary>
		/// <returns>String serialization</returns>
		public string Save()
		{
			return $"{CommonSerialization.delimiter}{CommonSerialization.delimiter}{this.storedSeed}";
		}

		public long GetInternalId()
		{
			return this.internalId;
		}
	}
}