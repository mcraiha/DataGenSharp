using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DatagenSharp
{
	/// <summary>
	/// Class for outputting Json data
	/// </summary>
	public class JsonOutput : IDataOutputter, ISerialization
	{
		public static readonly string LongName = "JsonOutput";

		public static readonly string ShortName = "JSON";
		
		public static readonly string Description = "Use this output if you want JSON";

		public static readonly string VersionNumber = "0.9";

		private static readonly List<Type> supportedParameterTypes = new List<Type>()
		{
			typeof(string)
		};

		private static readonly char separator = ',';
		private static readonly char quote = '"';

		private static readonly char colon = ':';

		private bool isFirstArrayElement = true;

		private StreamWriter output;

		private List<object> jsonAttributes = null;

		private long internalId = 0;

		public JsonOutput()
		{
			this.internalId = UniqueIdMaker.GetId();
		}

		public (bool success, string possibleError) Init(object parameter, Stream outputStream)
		{
			// Check if parameter type is supported
			if (parameter != null && !supportedParameterTypes.Contains(parameter.GetType()))
			{
				string error = ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType());
				return (success: false, possibleError: error);
			}

			this.output = new StreamWriter(outputStream, new UTF8Encoding(false));

			this.isFirstArrayElement = true;

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError) WriteHeader(List<object> entries)
		{
			this.jsonAttributes = entries;

			// No entries related header for JSON, but do start array
			this.output.WriteLine("[");

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError) WriteSingleEntry(List<object> entries)
		{
			if (this.isFirstArrayElement)
			{
				this.isFirstArrayElement = false;
			}
			else
			{
				this.output.Write(JsonOutput.separator);
			}

			bool firstEntry = true;
			int index = 0;

			foreach (var entry in entries)
			{
				if (!firstEntry)
				{
					this.output.Write(JsonOutput.separator);
					this.output.WriteLine();
				}
				else 
				{
					this.output.WriteLine("{");
					firstEntry = false;
				}

				// Write attribute
				this.output.Write($"{quote}{this.jsonAttributes[index].ToString()}{quote}{colon} ");
				index++;

				// Write value
				if (entry.GetType() == typeof(string))
				{
					this.output.Write($"{quote}{entry.ToString()}{quote}");
				}
				else
				{
					this.output.Write($"{entry.ToString()}");
				}
			}

			this.output.WriteLine("}");

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError) WriteFooter(List<object> parameter)
		{
			this.output.WriteLine();

			// End array
			this.output.WriteLine("]");

			this.output.Flush();
			this.output.Close();

			return (success: true, possibleError: "");
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}

		#region Serialization
		public (bool success, string possibleError) Load(string parameter)
		{
			return (true, "");
		}

		public string Save()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"{CommonSerialization.delimiter}NULL");

			return sb.ToString();
		}

		public long GetInternalId()
		{
			return this.internalId;
		}

		#endregion // Serialization
	}
}