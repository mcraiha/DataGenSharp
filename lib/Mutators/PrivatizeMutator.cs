using System;

namespace DatagenSharp
{
	public sealed class PrivatizeMutator : IMutator
	{
		public static readonly char defaultValueSeparator = ',';

		public static readonly string LongName = "PrivatizeMutator";

		public static readonly string ShortName = "PRIVATIZE";
		
		public static readonly string Description = "Only keep part of string and replace rest with chosen character";

		public static readonly string Examples = "passwordFFG -> 'pass*******'";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedInputTypes = new Type[] { typeof(string) };

		/// <summary>
		/// With this mutator supported output types is dynamic since output type can vary
		/// </summary>
		/// <returns>Output types that are always supported</returns>
		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string) };

		private int howManyToKeep = -1;
		private char replaceChar = ' ';

		public (bool success, string possibleError) Init(object parameter, int _)
		{
			if (parameter == null)
			{
				this.howManyToKeep = 0;
				this.replaceChar = '*';
			}
			else if (parameter.GetType() == typeof(string))
			{
				string[] splitted = ((string)parameter).Split(defaultValueSeparator);
				if (splitted.Length != 2)
				{
					return (success: false, $"{LongName} Init string needs to have {defaultValueSeparator} for separating true and false values!");
				}

				this.howManyToKeep = int.Parse(splitted[0]);
				this.replaceChar = splitted[1][0];
			}
			else
			{
				return (success: false, ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Mutate(object input, object parameter = null, Type wantedOutputType = null)
		{
			// Check that input is supported type
			if (Array.IndexOf(supportedInputTypes, input.GetType()) == -1)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedInputType(LongName, input.GetType()), result: null);
			}

			// Check that wanted output type is supported
			if (wantedOutputType != null && Array.IndexOf(this.GetSupportedOutputs(), wantedOutputType) == -1)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutputType), result: null);
			}

			string stringInput = (string)input;
			string returnValue = null;
			if (this.howManyToKeep < stringInput.Length)
			{
				if (this.howManyToKeep < 1)
				{
					returnValue = new string(this.replaceChar, stringInput.Length);
				}
				else
				{
					returnValue = stringInput.Substring(0, this.howManyToKeep) + new string(this.replaceChar, stringInput.Length - howManyToKeep);
				}
			}
			else
			{
				// No need to privatize anything
				returnValue = stringInput;
			}

			return (success: true, possibleError: "", result: returnValue);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedOutputTypes;
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}
	}
}