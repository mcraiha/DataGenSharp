using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class SomeSeparatedValueOutput : IDataOutputter
	{
		public static readonly string LongName = "SomeSeparatedValueOutput";
		
		public static readonly string Description = "Use this output if you want text where entries are separated by chars. e.g. comma-separated values (CSV) or tab-separated values (TSV)";

		public static readonly string VersionNumber = "0.9";

		private static readonly List<(string presetName, string separator, string[] dangerousEntries)> predefinedPresets = new List<(string, string, string[])>()
		{
			("csv", 	",", 	new[] {",", "\""}),
			("tsv", 	"\t", 	new[] {"\t"}),
		};

		private static readonly List<Type> supportedParameterTypes = new List<Type>()
		{
			typeof(string)
		};

		public static readonly List<(string parameter, int exclusiveCategory, string description)> supportedParameters = new List<(string parameter, int exclusiveCategory, string description)>()
		{
			("csv", 	0, 	"Set output to CSV (comma-separated values), see https://en.wikipedia.org/wiki/Comma-separated_values"),
			("tsv", 	0, 	"Set output to TSV (tab-separated values), see https://en.wikipedia.org/wiki/Tab-separated_values"),
		};

		private (string presetName, string separator, string[] dangerousEntries) preset;

		private StreamWriter output;

		private bool headerIsWanted = true;

		private bool lastLineShouldHaveNewLine = true;

		private bool writeBom = false;

		public (bool success, string possibleError) Init(object parameter, Stream outputStream)
		{
			// Check if parameter type is supported
			if (parameter != null && !supportedParameterTypes.Contains(parameter.GetType()))
			{
				string error = ErrorMessages.UnsupportedParameterType(LongName, parameter.GetType());
				return (success: false, possibleError: error);
			}

			if (writeBom)
			{
				this.output = new StreamWriter(outputStream, Encoding.UTF8);
			}
			else
			{
				this.output = new StreamWriter(outputStream, new UTF8Encoding(false));
			}

			this.preset = predefinedPresets[0];

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError) WriteHeader(List<object> entries)
		{
			if (!this.headerIsWanted)
			{
				return (success: true, possibleError: "");
			}

			bool firstEntry = true;

			foreach (var entry in entries)
			{
				if (!firstEntry)
				{
					this.output.Write(preset.separator);
				}

				string writeThis = entry.ToString();
				this.output.Write(writeThis);

				firstEntry = false;
			}

			this.output.WriteLine();

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError) WriteSingleEntry(List<object> entries)
		{
			bool firstEntry = true;

			foreach (var entry in entries)
			{
				if (!firstEntry)
				{
					this.output.Write(preset.separator);
				}

				string writeThis = entry.ToString();
				this.output.Write(writeThis);

				firstEntry = false;
			}

			this.output.WriteLine();

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError) WriteFooter(List<object> parameter)
		{
			this.output.Flush();
			this.output.Close();

			return (success: true, possibleError: "");
		}
	}
}