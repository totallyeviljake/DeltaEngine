using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// AssemblyChecker.IsAllowed is used whenever we have to check all loaded assemblies for types.
	/// Examples include BinaryDataExtensions and AutofacResolver.RegisterAllTypesFromAllAssemblies
	/// </summary>
	public class AssemblyCheckerTests : TestWithMocksOrVisually
	{
		[Test, Category("Slow")]
		public void MakeSureToOnlyIncludeAllowedDeltaEngineAndUserAssemblies()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var assembliesAllowed = new List<string>();
			foreach (Assembly assembly in assemblies.Where(assembly => assembly.IsAllowed()))
				assembliesAllowed.Add(assembly.GetName().Name);

			Assert.Greater(assembliesAllowed.Count, 0, "Assemblies: " + assembliesAllowed.ToText());
		}
	}
}