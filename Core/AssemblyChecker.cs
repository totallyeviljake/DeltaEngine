﻿using System;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Only used for Apps and visual tests, not for automated tests or NCrunch (slow & unsupported).
	/// </summary>
	public class AssemblyChecker
	{
		//ncrunch: no coverage start
		public AssemblyChecker(Assembly assembly)
		{
			assemblyName = assembly.GetName().Name;
		}

		private readonly string assemblyName;

		public bool IsAllowed
		{
			get { return !(IsMicrosoftAssembly() || IsIdeHelperTool() || IsThirdPartyLibrary()); }
		}

		protected bool IsMicrosoftAssembly()
		{
			return StartsWith("System", "mscorlib", "WindowsBase", "PresentationFramework",
				"PresentationCore", "WindowsFormsIntegration", "Microsoft.");
		}

		private bool StartsWith(params string[] partialNames)
		{
			return
				partialNames.Any(
					x => assemblyName.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
		}

		private bool IsIdeHelperTool()
		{
			return StartsWith("JetBrains.", "NUnit.Core.", "NCrunch.", "ReSharper.");
		}

		private bool IsThirdPartyLibrary()
		{
			return StartsWith("OpenAL32", "wrap_oal", "libEGL", "libgles", "libGLESv2", "libvlc",
				"libvlccore", "csogg", "csvorbis");
		}
	}
}