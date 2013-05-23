using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using Moq;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	internal class AutofacStarterForMockResolver : AutofacStarter
	{
		protected override void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return;

			RegisterInstance(new MockContentLoader(new AutofacContentDataResolver(this)));
			base.MakeSureContainerIsInitialized();
			foreach (var instance in registeredMocks)
				RegisterInstanceAsRunnerOrPresenterIfPossible(instance);

			testElapsedMs = GetTimeInMsForSlowTests();
		}

		private class MockContentLoader : ContentLoader
		{
			public MockContentLoader(ContentDataResolver resolver)
				: base(resolver)
			{
				contentFilenames.Add("Test", "Test.xml");
				contentFilenames.Add("DeltaEngineLogo", "DeltaEngineLogo.png");
			}

			protected override Stream GetContentDataStream(string contentName)
			{
				if (contentName == "Test")
					return new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream();
				return null;
			}
		}

		private readonly List<object> registeredMocks = new List<object>();

		private long testElapsedMs;

		private long GetTimeInMsForSlowTests()
		{
			if (testStarted != null)
				return testStarted.ElapsedMilliseconds;

			testStarted = new Stopwatch();
			testStarted.Start();
			return 0;
		}

		private Stopwatch testStarted;

		public Mock<T> RegisterMock<T>() where T : class
		{
			var mock = new Mock<T>();
			RegisterMock(mock.Object);
			return mock;
		}

		public T RegisterMock<T>(T instance) where T : class
		{
			Type instanceType = instance.GetType();
			foreach (object mock in registeredMocks.Where(mock => mock.GetType() == instanceType))
				throw new AssertionException("Unable to register mock " + instance +
					" because this type already has been registered: " + mock);

			registeredMocks.Add(instance);
			alreadyRegisteredTypes.AddRange(instanceType.GetInterfaces());
			RegisterInstance(instance);
			if (instance is Time)
				RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
			return instance;
		}

		public void AdvanceTimeAndExecuteRunners(float timeToAddInSeconds)
		{
			var simulateRunTicks = (int)Math.Round(timeToAddInSeconds * 60);
			for (int tick = 0; tick < simulateRunTicks; tick++)
			{
				RunAllRunners();
				RunAllPresenters();
			}
		}

		public override void Dispose()
		{
			WarnIfUnitTestTakesTooLong();
			ResetTimeToDefault();
			base.Dispose();
		}

		private static void ResetTimeToDefault()
		{
			Time.Current = new StopwatchTime();
		}

		//ncrunch: no coverage start
		private void WarnIfUnitTestTakesTooLong()
		{
			if (StackTraceExtensions.ContainsUnitTest() && TookLongerThan10Ms())
				Debug.WriteLine("This unit test takes too long (" + testElapsedMs + "ms, max. 10ms is " +
					"allowed), please add Category(\"Slow\") to run it nightly instead!");
		}

		private bool TookLongerThan10Ms()
		{
			testElapsedMs = GetTimeInMsForSlowTests() - testElapsedMs;
			return testElapsedMs > 10;
		}
	}
}