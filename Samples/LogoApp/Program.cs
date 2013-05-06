using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;

namespace LogoApp
{
	/// <summary>
	/// Displays a number of colored moving logo sprites bouncing around.
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<BouncingLogo, ContentLoader, EntitySystem>(NumberOfBouncingLogos);
		}

		private const int NumberOfBouncingLogos = 100;
	}
}