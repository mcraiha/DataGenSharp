using System;

namespace DatagenSharp
{
	public static class ErrorMessages
	{
		public static string UnsupportedNullParameter(string callerName, string where)
		{
			return $"{callerName} does not support null parameter!";
		}

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

		public static string ParameterArrayNotEnoughElements(string callerName, string where, int wanted, int has)
		{
			return $"{callerName} requires parameter array in {where} to have {wanted} elements, but current parameter has {has} objects!";
		}
	}
}