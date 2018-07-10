using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public static class GenerateData
	{
		public static GenerateChain chain = new GenerateChain();

		public static IDataOutputter output;

		public static object outputParameters = null;

		public static uint howManyStepToRun = 10;

		public static (bool success, string possibleError) Generate(Stream outputStream)
		{
			// Init output
			(bool initSuccess, string possibleInitError) = output.Init(outputParameters, outputStream);

			// Verify that init went OK
			if (!initSuccess)
			{
				return (success: initSuccess, possibleError: possibleInitError);
			}


			// Write header
			var names = chain.GetNames();
			output.WriteHeader(names.ConvertAll(s => (object)s));

			List<object> entries = null;

			// Write each entry
			for (int i = 0; i < howManyStepToRun; i++)
			{
				entries = GetCurrentLine();
				(bool entryWriteSuccess, string possibleEntryWriteError) = output.WriteSingleEntry(entries);

				if (!entryWriteSuccess)
				{
					return (success: entryWriteSuccess, possibleError: possibleEntryWriteError);
				}

				chain.UpdateToNextStep();
			}

			// Write footer
			return output.WriteFooter(entries);
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