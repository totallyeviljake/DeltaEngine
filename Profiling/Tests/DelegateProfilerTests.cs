using System;
using System.Threading;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Profiling.Tests
{
	public class DelegateProfilerTests
	{
		[Test, Category("Slow")]
		public void CheckTotalDurationOfDoingSomethingSlowAThousandTimes()
		{
			var profiler = new DelegateProfiler(() => Thread.Sleep(1));
			int totalDuration = profiler.TotalDurationInMilliseconds;
			Assert.IsTrue(totalDuration > 950);
			Console.WriteLine(totalDuration + " milliseconds for 1,000 iterations");
		}

		[Test, Category("Slow")]
		public void CheckIndividualDurationOfDoingSomethingSlowAThousandTimes()
		{
			var profiler = new DelegateProfiler(() => Thread.Sleep(1));
			int duration = profiler.AverageDurationInNanoseconds;
			Assert.IsTrue(duration > 950);
			Console.WriteLine(duration + " nanoseconds each");
		}

		[Test, Category("Slow")]
		public void CheckTotalDurationOfDoingSomethingFastAMillionTimes()
		{
			var profiler = new DelegateProfiler(DoSomeMaths, 1000000);
			int duration = profiler.TotalDurationInMilliseconds;
			Assert.IsTrue(duration > 5);
			Console.WriteLine(duration + " milliseconds for 1,000,000 iterations");
		}

		private static void DoSomeMaths()
		{
			MathExtensions.Sin(12345);
			MathExtensions.Cos(54321);
		}

		[Test, Category("Slow")]
		public void CheckIndividualDurationOfDoingSomethingFastAMillionTimes()
		{
			var profiler = new DelegateProfiler(DoSomeMaths, 1000000);
			int duration = profiler.AverageDurationInPicoseconds;
			Assert.IsTrue(duration > 5);
			Console.WriteLine(duration + " picoseconds each");
		}
	}
}