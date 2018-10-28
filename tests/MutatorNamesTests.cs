using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tests
{
	public class MutatorNamesTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void CompareMutatorNameUniqueness()
		{
			// UGLY HACK, since platform attribute does not work with dot net core + NUNIT, and core 2.0 tries to ComImport with Linux/Unix
			// https://github.com/dotnet/coreclr/issues/16804
			// https://github.com/dotnet/coreclr/issues/16804
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Assert.Pass();
			}

			// Arrange
			var interfaceType = typeof(IMutator);
			var all = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(x => x.GetTypes())
			.Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
			.Select(x => Activator.CreateInstance(x));

			HashSet<string> shortNames = new HashSet<string>();
			HashSet<string> longNames = new HashSet<string>();

			// Act
			foreach (var item in all)
			{
				var names = ((IMutator)item).GetNames();
				shortNames.Add(names.shortName);
				longNames.Add(names.longName);
			}

			// Assert
			Assert.Greater(all.Count(), 1, "There should be some mutators");
			Assert.AreEqual(all.Count(), shortNames.Count, "All shortnames should be unique");
			Assert.AreEqual(all.Count(), longNames.Count, "All longnames should be unique");
		}
	}
}