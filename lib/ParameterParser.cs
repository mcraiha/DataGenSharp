using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public static class ParameterParser
	{
		private static readonly char entrySeparator = '|';
		private static readonly char valueSeparator = '=';

		public static Dictionary<string, string> GetParameterDictionary(string parameter)
		{
			// By default we ignore case for key, so "someValue" and "SOMEVALUE" are equal
			Dictionary<string, string> returnDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

		public static bool ContainsKey(Dictionary<string, string> dict, List<string> parameterList)
		{
			if (dict == null || dict.Count < 1)
			{
				return false;
			}

			foreach (string parameter in parameterList)
			{
				if (dict.ContainsKey(parameter))
				{
					return true;
				}
			}

			return false;
		}
	}
}