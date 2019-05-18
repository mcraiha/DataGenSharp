using System;
using System.IO;
using System.Reflection;

namespace DatagenSharp
{
	/// <summary>
	/// Helper class for accesing embedded resources in binary
	/// </summary>
	public static class ResourceHelper
	{
		public static string[] GetAllResourceNames(Assembly assembly)
		{
			return assembly.GetManifestResourceNames();
		}

		public static bool CheckIfResourceExists(string resourceName, Assembly assembly)
		{
			string properResourceName = GetResourceName(assembly, resourceName);
			return (Array.IndexOf(assembly.GetManifestResourceNames(), properResourceName) > -1);
		}

		public static Stream LoadResourceStream(string resourceName, Assembly assembly)
		{
			string properResourceName = GetResourceName(assembly, resourceName);
			return assembly.GetManifestResourceStream(properResourceName);
		}

		private static string GetResourceName(Assembly assembly, string resourceName)
		{
			return $"{assembly.GetName().Name}.{resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".")}";
		}
	}
}