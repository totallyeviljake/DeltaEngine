using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaNinja.Entities;

namespace DeltaNinja
{
	class GameLogicsHandler : Behavior2D
	{
		public GameLogicsHandler(ScreenSpace screen, NumberFactory numberFactory)
		{
			this.numberFactory = numberFactory;
			this.screen = screen;
			this.waveTimeout = GameSettigs.InitialWaveTimeout;
		}

		private readonly ScreenSpace screen;
		private readonly NumberFactory numberFactory;

		private int waveIndex;
		private int waveCount;
		private int waveLogoCount;
		private float waveTimeout;

		public override void Handle(Entity2D entity)
		{
			var match = entity as Match;
			if (match == null || !match.IsActive)
			{
				// It seems that Handler can't be stopped
				waveIndex = 0;
				waveCount = 0;
				waveLogoCount = 0;
				waveTimeout = 0;
				return;
			}

			if (match.IsPaused) return;

			var time = Time.Current;

			foreach (var tip in match.PointsTips) tip.Fade();
			foreach (var flag in match.ErrorFlags) flag.Fade();

			Slice slice = match.Slice;

			if (slice.IsOver)
			{
				slice.Reset();
			}

			if (time.CheckEvery(waveTimeout))
				CheckWave(match);

			int points = 0;

			foreach (var logo in match.LogoArray)
			{
				bool toRemove = false;

				if (logo.IsSliced)
				{
					int logoPoints = (int) logo.Category;
					toRemove = true;

					if (logo.TopLeft.Y > screen.Viewport.Bottom - GameSettigs.CriticalHeight)
					{
						var data = logo.Get<SimplePhysics.Data>();

						if (data != null && data.Velocity.Y > 0)
							logoPoints += GameSettigs.CriticalBonus;
					}						
					
					match.PointsTips.Add(
						new PointsTip(numberFactory, logo.Center, logoPoints));

					match.AddOneMoreSlice();
					points += logoPoints;
				}
				else if (logo.IsOutside(screen.Viewport))
				{
					if (logo.Category != Logo.SizeCategory.Small)
					{
						match.ShowError(logo.TopLeft.X, logo.DrawArea.Width);
						if (!match.AddError()) return;
					}

					toRemove = true;
				}

				if (toRemove)
				{
					match.RemoveLogo(logo);
					logo.IsActive = false;
				}
			}

			match.AddPoints(points);
			match.ClearEntities();
		}

		public void CheckWave(Match match)
		{
			if (waveCount > 0)
			{
				match.CreateLogos(waveLogoCount);
				waveCount--;
				if (waveCount == 0) waveIndex++;
			}
			else if (match.LogoCount == 0)
			{
				var random = Randomizer.Current;

				int max = match.CurrentLevel + 1;
				int min = max < 4 ? 1 : max - 5;
				int count = random.Get(min, max);
				waveLogoCount = 1;

				switch (waveIndex)
				{
					case (0):
					case (4):
						waveCount = match.CurrentLevel;
						break;

					case (1):
						match.CreateLogos(count);
						waveIndex++;
						break;

					case (2):
						waveCount = max;
						break;

					case (3):
						match.CreateLogos(max < 5 ? max : 5);
						waveIndex++;					
						break;

					case (5):
						waveLogoCount = match.CurrentLevel % 3 == 0 ? 2 : 1;
						waveCount = count;
						break;

					case (6):
						match.NextLevel();
						waveTimeout -= 0.05f;
						waveIndex = 0;
						break;
				}
			}
		}

		public override Priority Priority
		{
			get { return Priority.First; }
		}
	}
}
