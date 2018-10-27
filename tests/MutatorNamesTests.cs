using NUnit.Framework;
using DatagenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

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