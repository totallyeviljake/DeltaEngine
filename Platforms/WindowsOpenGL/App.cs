﻿using System;
using System.Collections.Generic;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Provides the easily exchangeable App.Start function for OpenTK applications and visual tests.
	/// </summary>
	public class App
	{
		public void Start<AppEntryRunner>(int instancesToCreate = 1)
		{
			resolver.Start<AppEntryRunner>(instancesToCreate);
		}

		private readonly OpenTKResolver resolver = new OpenTKResolver();

		public void Start<AppEntryRunner, FirstClassToRegisterAndResolve>(int instancesToCreate = 1)
		{
			resolver.Start<AppEntryRunner, FirstClassToRegisterAndResolve>(instancesToCreate);
		}

		public void Start
			<AppEntryRunner, FirstClassToRegisterAndResolve, SecondClassToRegisterAndResolve>(
			int instancesToCreate = 1)
		{
			resolver.Start
				<AppEntryRunner, FirstClassToRegisterAndResolve, SecondClassToRegisterAndResolve>(
					instancesToCreate);
		}

		public void Start<AppEntryRunner>(IEnumerable<Type> typesToRegisterAndResolve,
			int instancesToCreate = 1)
		{
			resolver.Start<AppEntryRunner>(typesToRegisterAndResolve, instancesToCreate);
		}

		public void RegisterSingleton<T>()
		{
			resolver.RegisterSingleton<T>();
		}
	}
}