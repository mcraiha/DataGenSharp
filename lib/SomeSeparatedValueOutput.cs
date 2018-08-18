using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class SomeSeparatedValueOutput : IDataOutputter
	{
		public static readonly string LongName = "SomeSeparatedValueOutput";
		
		public static readonly string Description = "Use this output if you want text where entries are separated by chars. e.g. comma-separated values (CSV) or tab-separated values (TSV)";

		public static readonly string VersionNumber = "0.9.1";

		private static readonly List<(string presetName, string separator, string[] dangerousEntries)> predefinedPresets = new List<(string, string, string[])>()
		{
			("csv", 	",", 	new[] {",", "\"", "\r", "\n"}),
			("tsv", 	"\t", 	new[] {"\t", "\r", "\n"}),
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

		/// <summary>
		/// Chosen preset
		/// </summary>
		private (string presetName, string separator, string[] dangerousEntries) preset;

		/// <summary>
		/// Output for writing data
		/// </summary>
		private StreamWriter output;

		/// <summary>
		/// Is header line wanted
		/// </summary>
		private bool headerIsWanted = true;

		/// <summary>
		/// Should last line be new line
		/// </summary>
		private bool lastLineShouldHaveNewLine = true;

		/// <summary>
		/// By default we don't want to write BOM https://en.wikipedia.org/wiki/Byte_order_mark
		/// </summary>
		private bool writeBom = false;

		public (bool success, string possibleError) Init(object parameter, Stream outputStream)
		{
			// Check if parameter type is supported
			if (parameter != null && !supportedParameterTypes.Contains(parameter.GetType()))
			{
				string error = ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType());
				return (success: false, possibleError: error);
			}
			else if (parameter == null)
			{
				// If no parameters are given then CSV is written
				this.preset = predefinedPresets[0];
				this.writeBom = false;
			}
			else
			{
				string stringParameter = (string)parameter;
				Dictionary<string, object> paramDict = ParameterParser.GetParameterDictionary(stringParameter);

				// First preset
				string[] presetNames = SomeSeparatedValueOutput.predefinedPresets.Select(x => x.presetName).ToArray();
				foreach (string presetName in presetNames)
				{
					if (paramDict.ContainsKey(presetName))
					{
						this.preset = SomeSeparatedValueOutput.predefinedPresets.Find(x => x.presetName == presetName);
						break;
					}
				}

				// Then is header wanted
				if (paramDict.ContainsKey(nameof(headerIsWanted)))
				{
					this.headerIsWanted = (bool)paramDict[nameof(headerIsWanted)];
				}

				// Then if last line should have new line
				if (paramDict.ContainsKey(nameof(lastLineShouldHaveNewLine)))
				{
					this.lastLineShouldHaveNewLine = (bool)paramDict[nameof(lastLineShouldHaveNewLine)];
				}

				// Finally if BOM should be used
				if (paramDict.ContainsKey(nameof(writeBom)))
				{
					this.writeBom = (bool)paramDict[nameof(writeBom)];
				}
			}

			if (this.writeBom)
			{
				this.output = new StreamWriter(outputStream, Encoding.UTF8);
			}
			else
			{
				this.output = new StreamWriter(outputStream, new UTF8Encoding(false));
			}	

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