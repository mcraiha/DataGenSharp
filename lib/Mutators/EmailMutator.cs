using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatagenSharp
{
	public class EmailMutator : IMutator
	{
		private static readonly List<string> emailDomains = new List<string>()
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

		private static readonly char atSeparator = '@';
		private static readonly char defaultNameSeparator = '.';

		public static readonly string LongName = "EmailMutator";
		
		public static readonly string Description = "Turns input to email address";

		public static readonly string Examples = "'John Smith' -> 'john.smith@gmail.com'";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedInputTypes = new Type[] { typeof(string) };

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string) };

		private Random rng;


		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			// TODO: Add some parameters here (keep case, name separator override, add single doman mode etc.)
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

			string domain = emailDomains[this.rng.Next(0, emailDomains.Count)];

			object returnValue = $"{ModifyForEmail(tempString)}{atSeparator}{domain}";

			return (success: true, possibleError: "", result: returnValue);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedOutputTypes;
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