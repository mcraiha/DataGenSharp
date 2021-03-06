using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public static class ParameterParser
	{
		#region String
		public static readonly char entrySeparator = '|';
		public static readonly char valueSeparator = '=';

		public static Dictionary<string, object> GetParameterDictionary(string parameter)
		{
			// By default we ignore case for key, so "someValue" and "SOMEVALUE" are equal
			Dictionary<string, object> returnDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

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
					returnDictionary[entry] = true; 
				}
			}

			return returnDictionary;
		}

		public static bool ContainsKey(Dictionary<string, object> dict, List<string> parameterList)
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

		public static string GetValueWithKeys(Dictionary<string, object> dict, List<string> parameterList)
		{
			if (dict == null || dict.Count < 1)
			{
				return null;
			}

			foreach (string parameter in parameterList)
			{
				if (dict.ContainsKey(parameter))
				{
					return (string)dict[parameter];
				}
			}

			return null;
		}

		#endregion // String


		#region Objects

		/// <summary>
		/// Check if object is an array, and in that case get type of elements it contains
		/// </summary>
		/// <param name="isArray">Is Array</param>
		/// <param name="elementType">Element Type</param>
		/// <returns>Valuetuple which contains the information</returns>
		public static (bool isArray, Type elementType) PrecheckPossibleArray(object input)
		{
			Type valueType = input.GetType();
			if (!valueType.IsArray)
			{
				return (isArray: false, elementType: null);
			}

			return (isArray: true, elementType: valueType.GetElementType());
		}

		#endregion // Objects
	}
}