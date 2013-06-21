using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace SideScrollerSample
{
	/// <summary>
	/// Initialization of the SideScrollerGame, creation of all further instances used during the game.
	/// </summary>
	internal class SideScrollerGame : Entity
	{
		public SideScrollerGame(ContentLoader loader, InputCommands commands)
		{
			interact = new InteractionLogics();
			playerTexture = loader.Load<Image>("testplane");
			enemyTexture = loader.Load<Image>("testplaneFlip");
			player = new PlayerPlane(playerTexture, new Point(0.15f, 0.5f));
			controls = new GameControls(commands);
			//background = new ParallaxBackground(loader, screen);
			BindPlayerToControls();
			BindPlayerAndInteraction();
			Add<EnemySpawner>();
		}

		internal readonly PlayerPlane player;
		internal readonly GameControls controls;
		internal readonly InteractionLogics interact;
		internal Image playerTexture, enemyTexture;

		//internal ParallaxBackground background;

		public void CreateEnemyAtPosition(Point position)
		{
			var enemy = new EnemyPlane(enemyTexture, position);
			player.PlayerFiredShot += point => enemy.CheckIfHitAndReact(point);
			enemy.EnemyFiredShot += point => interact.FireShotByEnemy(point);
		}

		private void BindPlayerToControls()
		{
			controls.Ascend += () => { player.AccelerateVertically(-Time.Current.Delta); };
			controls.Sink += () => { player.AccelerateVertically(Time.Current.Delta); };
			controls.VerticalStop += () => { player.StopVertically(); };
			controls.Fire += () => { player.IsFireing = true; };
			controls.HoldFire += () => { player.IsFireing = false; };
			controls.SlowDown += () => { };
			controls.Accelerate += () => { };
		}

		private void BindPlayerAndInteraction()
		{
			player.PlayerFiredShot += point => { interact.FireShotByPlayer(point); };
		}

		private class EnemySpawner : EntityHandler
		{
			public EnemySpawner(ScreenSpace screenSpace)
			{
				this.screenSpace = screenSpace;
				Filter = entity => entity is SideScrollerGame;
			}

			public override void Handle(Entity entity)
			{
				var game = entity as SideScrollerGame;
				if (Time.Current.Milliseconds - timeLastOneSpawned > 2000)
				{
					game.CreateEnemyAtPosition(new Point(screenSpace.Viewport.Right,
						screenSpace.Viewport.Center.Y + alternating * 0.1f));
					timeLastOneSpawned = Time.Current.Milliseconds;
					alternating *= -1;
				}
			}

			private float timeLastOneSpawned;
			private int alternating = 1;
			private readonly ScreenSpace screenSpace;
		}
	}
}