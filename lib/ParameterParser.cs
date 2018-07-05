using System.Collections.Generic;

namespace DatagenSharp
{
	public static class ParameterParser
	{
		private static readonly char entrySeparator = '|';
		private static readonly char valueSeparator = '=';

		public static Dictionary<string, string> GetParameterDictionary(string parameter)
		{
			Dictionary<string, string> returnDictionary = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(parameter))
			{
				return returnDictionary;
			}

			string[] entries = parameter.Split(entrySeparator);

			foreach (string entry in entries)
			{
				if (entry.IndexOf(valueSeparator) != -1)
				{
					string[] keyAndValue = entry.Split(valueSeparator);
					returnDictionary[keyAndValue[0]] = keyAndValue[1];
				}
				else
				{
					returnDictionary[entry] = true.ToString(); 
				}
			}

			return returnDictionary;
		}
	}
}