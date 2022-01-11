using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DatagenSharp
{
	public sealed class EmailMutator : IMutator
	{
		private static readonly List<string> emailValidDomains = new List<string>()
		{
			"gmail.com",
			"yahoo.com",
			"hotmail.com",
			"aol.com",
			"msn.com",
			"wanadoo.fr",
			"comcast.net",
			"live.com",
			"rediffmail.com",
			"free.fr",
			"gmx.de",
			"web.de",
			"yandex.ru",
			"libero.it",	
		};

		// These are picked from RFC 2606 
		private static readonly List<string> emailExampleDomains = new List<string>()
		{
			"example.com",
			"example.net",
			"example.org",
		};

		private static readonly char atSeparator = '@';
		private static readonly char defaultNameSeparator = '.';

		public static readonly string LongName = "EmailMutator";

		public static readonly string ShortName = "EMAIL";
		
		public static readonly string Description = "Turns input to email address";

		public static readonly string Examples = "'John Smith' -> 'john.smith@gmail.com'";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedInputTypes = new Type[] { typeof(string) };

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string) };

		private Random rng;

		/// <summary>
		/// Should mutator only produce example domain names, e.g. john.doe@example.com
		/// </summary>
		private bool onlyDoExamples = false;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			// TODO: Add some parameters here (keep case, name separator override, add single domain mode etc.)
			if (parameter != null)
			{
				if (parameter.GetType() == typeof(string))
				{
					if ("EXAMPLES" == (string)parameter)
					{
						this.onlyDoExamples = true;
					}
				}
			}

			this.rng = new Random(seed);
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
			if (wantedOutputType != null && Array.IndexOf(supportedOutputTypes, wantedOutputType) == -1)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutputType), result: null);
			}

			string tempString = (string)input;

			string domain = this.onlyDoExamples ? emailExampleDomains[this.rng.Next(0, emailExampleDomains.Count)] : emailValidDomains[this.rng.Next(0, emailValidDomains.Count)];

			object returnValue = $"{ModifyForEmail(tempString)}{atSeparator}{domain}";

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
		
		private static string ModifyForEmail(string input)
		{
			// Remove whitespace from start and end
			string tempValue = input.Trim();

			// Replace rest of the whitespace chars with wanted char
			tempValue = Regex.Replace(tempValue, @"\s", defaultNameSeparator.ToString());

			return tempValue;
		}
	}
}