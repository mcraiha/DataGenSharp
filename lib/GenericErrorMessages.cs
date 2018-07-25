using System;

namespace DatagenSharp
{
	public static class ErrorMessages
	{
		public static string UnsupportedParameterType(string callerName, string where, Type unsupportedType)
		{
			return $"{callerName} does not support {unsupportedType} as {where} parameter type!";
		}

		public static string UnsupportedParameterValue(string callerName, string where, string unsupportedValue)
		{
			return $"{callerName} does not support {unsupportedValue} as {where} parameter value!";
		}

		public static string UnsupportedWantedOutputType(string callerName, Type unsupportedOutputType)
		{
			return $"{callerName} does not support {unsupportedOutputType} as output type!";
		}

		public static string UnsupportedInputType(string callerName, Type unsupportedInputType)
		{
			return $"{callerName} does not support {unsupportedInputType} as input type!";
		}
	}
}