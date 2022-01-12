using System;
using System.Text;
using System.Collections.Generic;

namespace DatagenSharp
{
	public sealed class GuidGenerator : IDataGenerator
	{
		public static readonly string LongName = "GuidGenerator";

		public static readonly string ShortName = "GUID";
		
		public static readonly string Description = "Generate GUID (globally unique identifier) values (e.g. 123e4567-e89b-12d3-a456-426655440000)";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string) };

		private static readonly List<string> noGroupingKeywords = new List<string>() { "nogroups", "joined" };

		public static readonly char groupSeparator = '-';

		private Random rng = null;

		/// <summary>
		/// Add group separators, (when enabled output is like 6800f4c7-435e-02aa-c27c-4fd927fc2227, and when disabled the output is like )
		/// </summary>
		private bool addGroupSeparators = true;

		private byte[] currentValue = new byte[16];

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			this.rng = new Random(seed);

			if (parameter == null)
			{

			}
			else if (parameter.GetType() == typeof(string))
			{
				string parameterAsString = (string)parameter;
				parameterAsString = parameterAsString.ToLower();

				if (noGroupingKeywords.Contains(parameterAsString))
				{
					this.addGroupSeparators = false;
				}
			}

			// Generate first value
			this.NextStep();

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			if (wantedOutput == null)
			{

			}
			else if (Array.IndexOf(supportedOutputTypes, wantedOutput) > -1)
			{
				
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutput), result: null);
			}

			object returnValue = FormatGuid(this.currentValue, this.addGroupSeparators);

			return (success: true, possibleError: "", result: returnValue);
		}

		private static string FormatGuid(byte[] bytes, bool useGrouping)
		{
			StringBuilder hex = new StringBuilder(bytes.Length * 2);

			int index = 0;
			foreach (byte b in bytes)
			{
				hex.AppendFormat("{0:x2}", b);
				index++;
				if (useGrouping && (index == 4 || index == 6 || index == 8 || index == 10))
				{
					hex.Append(groupSeparator);
				}
			}
    
  			return hex.ToString();
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
	}
}