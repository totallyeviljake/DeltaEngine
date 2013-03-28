﻿using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class GameTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Paddle paddle, RelativeScreenSpace screen, Game game) => { });
		}

		[VisualTest]
		public void CreateBreakoutGameManually(Type type)
		{
			Start(type, (Paddle paddle, AutofacStarter app, RelativeScreenSpace screen) =>
			{
				app.Resolve<BallInLevel>();
				app.Resolve<Level>();
				app.Resolve<Background>();
				app.Resolve<InputCommands>().Add(Key.Escape, app.Close);
			});
		}

		[VisualTest]
		public void RemoveBallIfGameIsOver(Type type)
		{
			Start(type, (Game game, Score score) =>
			{
				bool isGameOver = false;
				score.GameOver += () => isGameOver = true;
				score.LifeLost();
				score.LifeLost();
				score.LifeLost();
				Assert.IsTrue(isGameOver);
			});
		}

		[VisualTest]
		public void UpdateScore(Type type)
		{
			Start(type, (Game game, UI ui, Window window) =>
			{
				if (testResolver != null)
					testResolver.AdvanceTimeAndExecuteRunners(0.2f);

				Assert.IsTrue(window.Title.Contains("Breakout Level:"));
			});
		}

		[Test]
		public void KillingAllBricksShouldAdvanceToNextLevel()
		{
			Score remScore = null;
			bool isGameOver = false;
			Start(typeof(TestResolver), (Game game, Level level, Score score) =>
			{
				remScore = score;
				remScore.GameOver += () => isGameOver = true;
				Assert.AreEqual(1, score.Level);
				DisposeAllBricks(level);
				Assert.AreEqual(0, level.BricksLeft);
			});

			Assert.AreEqual(2, remScore.Level);
			Assert.IsFalse(isGameOver);
		}

		private static void DisposeAllBricks(Level level)
		{
			for (float x = 0; x < 1.0f; x += 0.1f)
				for (float y = 0; y < 1.0f; y += 0.1f)
					if (level.GetBrickAt(x, y) != null)
						level.GetBrickAt(x, y).Dispose();
		}

		[VisualTest]
		public void ResurrectBricksRandomly(Type type)
		{
			LevelWithRessurrection level = null;
			Start(type, (Game game, LevelWithRessurrection l) => { level = l; }, () =>
			{
				if (testResolver == null)
					return;

				testResolver.SetKeyboardState(Key.Space, State.Pressing);
				level.GetBrickAt(0.25f, 0.125f).Dispose();
				level.GetBrickAt(0.75f, 0.125f).Dispose();
				level.GetBrickAt(0.25f, 0.375f).Dispose();
				testResolver.AdvanceTimeAndExecuteRunners(4.0f);
			});
		}

		private class LevelWithRessurrection : Level, Runner<Time>
		{
			public LevelWithRessurrection(Content content, Renderer renderer, Score score)
				: base(content, renderer, score) { }

			public void Run(Time time)
			{
				if (!time.CheckEvery(2))
					return;

				var random = new PseudoRandom();
				var brick = bricks[random.Get(0, rows), random.Get(0, columns)];
				if (brick == null || brick.IsVisible)
					return;

				brick.IsVisible = true;
				renderer.Add(brick);
			}
		}

		[IntegrationTest]
		public void GoFullscreen(Type resolver)
		{
			Start(resolver, (RelativeScreenSpace screen, Game game, Window window) =>
			{
				var fullscreenResolution = new Size(1920, 1080);
				window.SetFullscreen(fullscreenResolution);
				Assert.IsTrue(window.IsFullscreen);
				Assert.AreEqual(fullscreenResolution, window.TotalPixelSize);
			});
		}
	}
}