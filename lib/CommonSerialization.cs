using System.Linq;


namespace DatagenSharp
{
	/// <summary>
	/// Common (de)serialization related stuff
	/// </summary>
	public static class CommonSerialization
	{
		/// <summary>
		/// Each object definition line should start with this character
		/// </summary>
		public static readonly char itemStartChar = '-';

		/// <summary>
		/// Delimiter for values of single object
		/// </summary>
		public static readonly char delimiter = '~';

		public static readonly string nullValue = "NULL";

		public static bool IsSomewhatValidGeneratorSaveData(string data)
		{
			return CountCertainChars(data, delimiter) == 2;
		}

		private static int CountCertainChars(string input, char countThese)
		{
			int returnValue = 0;

			foreach (char c in input)
			{
				if (countThese == c)
				{
					returnValue++;
				}
			}

			return returnValue;
		}

		public static string[] SplitGeneratorSaveData(string data)
		{
			return data.Split(delimiter).Skip(1).ToArray();
		}
	}
}