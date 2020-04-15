using System;
using System.Collections.Generic;
using System.Text;

namespace DatagenSharp
{
	public sealed class BitcoinAddressGenerator : IDataGenerator, ISerialization
	{
		public static readonly string LongName = "BitcoinAddressGenerator";
		public static readonly string ShortName = "BTC";
		
		public static readonly string Description = "Generate Bitcoin addresses (e.g. 1VLA7S6JPGSEP6DOZE9RBDN16IC860 )";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string) };

		private Random rng = null;

		private string currentValue = null;

		/// <summary>
		/// Stored seed, needed only for serialization purposes because there is no easy way to get seed back from Random
		/// </summary>
		private int storedSeed = 0;

		private long internalId = 0;

		public BitcoinAddressGenerator()
		{
			this.internalId = UniqueIdMaker.GetId();
		}

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{

			}
			else if (parameter.GetType() == typeof(string))
			{
				if (string.IsNullOrEmpty((string)parameter))
				{

				}
				else
				{
					return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
				}
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			this.rng = new Random(seed);

			this.storedSeed = seed;

			this.NextStep();

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			object returnValue = this.currentValue;

			if (wantedOutput == null || wantedOutput == typeof(string))
			{

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
			// Only common P2PKH type BTC addresses are supported, see https://en.bitcoinwiki.org/wiki/Bitcoin_address
			int randomLength = this.rng.Next(26, 34);
			this.currentValue = "1" + GenerateBase58(this.rng, randomLength);
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		/// <summary>
		/// Deserialize BitcoinAddressGenerator
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
		/// Serialize BitcoinAddressGenerator
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

		private static readonly string validBase58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
		private static string GenerateBase58(Random rng, int length)
		{
			StringBuilder sb = new StringBuilder(length);

			for (int i = 0; i< length; i++)
			{
				int index = rng.Next(0, validBase58Chars.Length);
				sb.Append(validBase58Chars[index]);
			}

			return sb.ToString();
		}
	}
}