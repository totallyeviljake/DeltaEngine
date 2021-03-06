using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Controller
	/// </summary>
	public class ControllerTests : TestWithMocksOrVisually
	{
		public void Initialize(ScreenSpace screen)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f ? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
			controller = new Controller(displayMode, content);
			sounds = controller.Get<Soundbank>();
			grid = controller.Get<Grid>();
		}

		private Orientation displayMode;
		private IDisposable fixedRandomScope;
		private Controller controller;
		private JewelBlocksContent content;
		private Soundbank sounds;
		private Grid grid;

		[Test]
		public void RunCreatesFallingAndUpcomingBlocks()
		{
			Initialize(Resolve<ScreenSpace>());
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsNotNull(controller.FallingBlock);
			Assert.IsNotNull(controller.UpcomingBlock);
		}

		[Test]
		public void DropSlowAffixesBlocksSlowly()
		{
			Initialize(Resolve<ScreenSpace>());
			controller.IsFallingFast = false;
			controller.FallingBlock = new Block(displayMode, content, Point.Zero);
			controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);

			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreEqual(0, CountBricks(grid));
			resolver.AdvanceTimeAndExecuteRunners(1.5f);
			Assert.AreEqual(0, CountBricks(grid));
			resolver.AdvanceTimeAndExecuteRunners(9.0f);
			Assert.AreEqual(4, CountBricks(grid), 1);
		}

		internal static int CountBricks(Grid grid)
		{
			int count = 0;
			foreach (var brick in grid.bricks)
				if (brick != null)
					count++;

			return count;
		}

		[Test]
		public void DropFastAffixesBlocksQuickly()
		{
			Initialize(Resolve<ScreenSpace>());
			controller.IsFallingFast = true;
			controller.FallingBlock = new Block(displayMode, content, Point.Zero);
			controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);

			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreEqual(0, CountBricks(grid));
			resolver.AdvanceTimeAndExecuteRunners(1.4f);
			Assert.AreEqual(4, CountBricks(grid), 1);
		}

		[Test]
		public void ABlockAffixingPlaysASound()
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.BlockAffixed.IsAnyInstancePlaying);
			resolver.AdvanceTimeAndExecuteRunners(12.0f);
			Assert.IsTrue(sounds.BlockAffixed.IsAnyInstancePlaying);
		}

		[Test]
		public void RunScoresPointsOverTime()
		{
			Initialize(Resolve<ScreenSpace>());
			int score = 0;
			controller.AddToScore += points => score += points;
			controller.FallingBlock = new Block(displayMode, content, Point.Zero);
			controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreEqual(1, score);
			resolver.AdvanceTimeAndExecuteRunners(9.0f);
			Assert.AreEqual(2, score);
		}

		[Test]
		public void WhenABlockAffixesTheUpcomingBlockBecomesTheFallingBlock()
		{
			Initialize(Resolve<ScreenSpace>());
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			var upcomingBlock = controller.UpcomingBlock;
			resolver.AdvanceTimeAndExecuteRunners(10.0f);
			Assert.AreEqual(upcomingBlock, controller.FallingBlock);
		}

		[Test]
		public void CantMoveLeftAtLeftWall(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.BlockCouldntMove.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Point(0, 1));
			controller.MoveBlockLeftIfPossible();
			Assert.IsTrue(sounds.BlockCouldntMove.IsAnyInstancePlaying);
			Assert.AreEqual(0, controller.FallingBlock.Left);
		}

		[Test, Category("Slow")]
		public void CanMoveLeftElsewhere(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Point(3, 1));
			controller.MoveBlockLeftIfPossible();
			Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
			Assert.AreEqual(2, controller.FallingBlock.Left);
		}

		[Test, Category("Slow")]
		public void CantMoveRightAtRightWall(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.BlockCouldntMove.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Point(11, 1));
			controller.MoveBlockRightIfPossible();
			Assert.AreEqual(11, controller.FallingBlock.Left);
			Assert.IsTrue(sounds.BlockCouldntMove.IsAnyInstancePlaying);
		}

		[Test, Category("Slow")]
		public void CanMoveRightElsewhere(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Point(3, 1));
			controller.MoveBlockRightIfPossible();
			Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
			Assert.AreEqual(4, controller.FallingBlock.Left);
		}

		[Test, Category("Slow")]
		public void RotateClockwise(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Point(8, 1));
			controller.RotateBlockAntiClockwiseIfPossible();
			Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock.Left = 11;
			controller.RotateBlockAntiClockwiseIfPossible();
		}

		[Test, Category("Slow")]
		public void LoseIfIsBrickOnTopRow()
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(sounds.GameLost.IsAnyInstancePlaying);
			grid.AffixBlock(new Block(displayMode, content, new Point(1, 0)));
			bool lost = false;
			controller.Lose += () => lost = true;
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsTrue(lost);
			Assert.IsTrue(sounds.GameLost.IsAnyInstancePlaying);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			fixedRandomScope = Randomizer.Use(new FixedRandom());
			fixedRandomScope.Dispose();
		}
	}
}