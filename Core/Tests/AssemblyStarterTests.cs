using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class AssemblyStarterTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void StartRunTimeTestInAppDomain()
		{
			using (var starter = new AssemblyStarter("DeltaEngine.Core.Tests.dll"))
				starter.Start("TimeTests", "RunTime", null);
		}
	}
}