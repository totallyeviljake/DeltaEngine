using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaNinja.Entities;
using NUnit.Framework;

namespace DeltaNinja.Tests.Entities
{
	internal class LogoTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateLogos()
		{
			var randomizer = new PseudoRandom();
			var factory = new LogoFactory(Resolve<ScreenSpace>());
			var logos = new List<Logo>();

			var n = randomizer.Get(10, 100);
			for (int i = 0; i < n; i++)
			{
				var logo = factory.Create();
				if (logo != null)
					logos.Add(logo);
			}

			Assert.IsTrue(logos.Count == n);
		}

		[Test]
		public void ShowLogosAndWait()
		{
			var screen = Resolve<ScreenSpace>();
			var factory = new LogoFactory(screen);
			var logos = new List<Logo>();

			var n = 10; // randomizer.Get(10, 100);
			for (int i = 0; i < n; i++)
			{
				var logo = factory.Create();
				if (logo != null)
					logos.Add(logo);
			}

			Assert.IsTrue(logos.Count == n);

			if (resolver.GetType() != typeof(MockResolver)) return;
			var time = Time.Current;

			while (time.Milliseconds < 10000)
			{
				var mouse = Resolve<MockMouse>();
				mouse.SetButtonState(MouseButton.Left, State.Releasing);
				resolver.AdvanceTimeAndExecuteRunners(1);

				if (time.CheckEvery(1))
				{
					screen.Window.Title = "Logo count: " + logos.Count;

					logos.RemoveAll(x => x.IsOutside(screen.Viewport));
				}
			}

			Assert.IsTrue(logos.Count == 0);
			screen.Window.Dispose();					
		}
	}
}