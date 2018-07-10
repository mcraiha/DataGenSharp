using System;

namespace DatagenSharp
{
	public static class ErrorMessages
	{
		public static string UnsupportedParameterType(string callerName, string where, Type unsupportedType)
		{
			return $"{callerName} does not support {unsupportedType} as {where} paramater type!";
		} 
	}
}