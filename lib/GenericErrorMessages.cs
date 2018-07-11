using System;

namespace DatagenSharp
{
	public static class ErrorMessages
	{
		public static string UnsupportedParameterType(string callerName, string where, Type unsupportedType)
		{
			return $"{callerName} does not support {unsupportedType} as {where} paramater type!";
		}

		public static string UnsupportedWantedOutputType(string callerName, Type unsupportedOutputType)
		{
			return $"{callerName} does not support {unsupportedOutputType} as output type!";
		}
	}
}