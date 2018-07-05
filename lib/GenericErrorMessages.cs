using System;

namespace DatagenSharp
{
	public static class ErrorMessages
	{
		public static string UnsupportedParameterType(string callerName, Type unsupportedType)
		{
			return $"{callerName} does not support {unsupportedType} as paramater type!";
		} 
	}
}