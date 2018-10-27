using System;
using System.Collections.Generic;
using System.Text;

namespace DatagenSharp
{
	public class BitcoinAddressGenerator : IDataGenerator
	{
		public static readonly string LongName = "BitcoinAddressGenerator";
		public static readonly string ShortName = "BTC";
		
		public static readonly string Description = "Generate Bitcoin addresses (e.g. 1VLA7S6JPGSEP6DOZE9RBDN16IC860 )";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string) };

		private Random rng = null;

		private string currentValue = null;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{

			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			this.rng = new Random(seed);

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