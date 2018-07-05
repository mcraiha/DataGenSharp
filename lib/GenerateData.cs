using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public static class GenerateData
	{
		public static GenerateChain chain = new GenerateChain();

		public static IDataOutputter output;

		public static object outputParameters;

		public static uint howManyStepToRun = 10;

		public static void Generate(Stream outputStream)
		{
			// Init output
			output.Init(outputParameters, outputStream);

			// Write header
			var names = chain.GetNames();
			output.WriteHeader(names.ConvertAll(s => (object)s));

			List<object> entries = null;

			// Write each entry
			for (int i = 0; i < howManyStepToRun; i++)
			{
				entries = GetCurrentLine();
				output.WriteSingleEntry(entries);
				chain.UpdateToNextStep();
			}

			// Write footer
			output.WriteFooter(entries);
		}

		private static List<object> GetCurrentLine()
		{
			return chain.RunOneStep();
		}

		public static void Load(string parameter)
		{

		}

		public static string Save()
		{
			return "";
		}
	}
}