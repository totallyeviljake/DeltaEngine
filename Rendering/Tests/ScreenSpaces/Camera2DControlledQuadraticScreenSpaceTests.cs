using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.ScreenSpaces
{
	public class Camera2DControlledQuadraticScreenSpaceTests : TestWithAllFrameworks
	{
		[Test]
		public void LookAt()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new MockResolver().rendering.Window);
			Assert.AreEqual(Point.Half, camera.LookAt);
			camera.LookAt = Point.One;
			Assert.AreEqual(Point.One, camera.LookAt);
		}

		[Test]
		public void Zoom()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new MockResolver().rendering.Window);
			Assert.AreEqual(1.0f, camera.Zoom);
			camera.Zoom = 2.5f;
			Assert.AreEqual(2.5f, camera.Zoom);
		}

		[Test]
		public void IfCameraNotAdjustedItBehavesIdenticallyToQuadraticScreenSpace()
		{
			Start(typeof(MockResolver),
				(QuadraticScreenSpace q, Camera2DControlledQuadraticScreenSpace c) =>
				{
					Assert.AreEqual(q.FromPixelSpace(new Point(1, 2)), c.FromPixelSpace(new Point(1, 2)));
					Assert.AreEqual(q.FromPixelSpace(new Size(-3, 4)), c.FromPixelSpace(new Size(-3, 4)));
					Assert.AreEqual(q.ToPixelSpace(new Point(2, 6)), c.ToPixelSpace(new Point(2, 6)));
					Assert.AreEqual(q.ToPixelSpace(new Size(-2, 0)), c.ToPixelSpace(new Size(-2, 0)));
				});
		}

		[Test]
		public void IfCameraNotAdjustedEdgesAreIdenticalToQuadraticScreenSpace()
		{
			Start(typeof(MockResolver),
				(QuadraticScreenSpace q, Camera2DControlledQuadraticScreenSpace c) =>
				{
					Assert.AreEqual(q.TopLeft, c.TopLeft);
					Assert.AreEqual(q.BottomRight, c.BottomRight);
					Assert.AreEqual(q.Top, c.Top, 0.0001f);
					Assert.AreEqual(q.Left, c.Left, 0.0001f);
					Assert.AreEqual(q.Bottom, c.Bottom, 0.0001f);
					Assert.AreEqual(q.Right, c.Right, 0.0001f);
				});
		}

		[Test]
		public void EdgesAfterZoomingIn()
		{
			var window = new MockResolver().rendering.Window;
			Assert.AreEqual(16 / 9.0f, window.ViewportPixelSize.AspectRatio);
			var camera = new Camera2DControlledQuadraticScreenSpace(window) { Zoom = 2.0f };

			Assert.AreEqual(new Point(0.25f, 0.359375f), camera.TopLeft);
			Assert.AreEqual(new Point(0.75f, 0.640625f), camera.BottomRight);
			Assert.AreEqual(0.359375f, camera.Top, 0.0001f);
			Assert.AreEqual(0.25f, camera.Left, 0.0001f);
			Assert.AreEqual(0.640625f, camera.Bottom, 0.0001f);
			Assert.AreEqual(0.75f, camera.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterPanning()
		{
			var window = new MockResolver().rendering.Window;
			Assert.AreEqual(16 / 9.0f, window.ViewportPixelSize.AspectRatio);
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(0.75f, 0.6f)
			};

			Assert.AreEqual(new Point(0.25f, 0.31875f), camera.TopLeft);
			Assert.AreEqual(new Point(1.25f, 0.88125f), camera.BottomRight);
			Assert.AreEqual(0.31875f, camera.Top, 0.0001f);
			Assert.AreEqual(0.25f, camera.Left, 0.0001f);
			Assert.AreEqual(0.88125f, camera.Bottom, 0.0001f);
			Assert.AreEqual(1.25f, camera.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterPanningAndZooming()
		{
			var window = new MockResolver().rendering.Window;
			Assert.AreEqual(16 / 9.0f, window.ViewportPixelSize.AspectRatio);
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(0.4f, 0.5f),
				Zoom = 0.5f
			};

			Assert.AreEqual(new Point(-0.6f, -0.0625f), camera.TopLeft);
			Assert.AreEqual(new Point(1.4f, 1.0625f), camera.BottomRight);
			Assert.AreEqual(-0.0625f, camera.Top, 0.0001f);
			Assert.AreEqual(-0.6f, camera.Left, 0.0001f);
			Assert.AreEqual(1.0625f, camera.Bottom, 0.0001f);
			Assert.AreEqual(1.4f, camera.Right, 0.0001f);
		}

		[Test]
		public void LoopingToAndFromPixelSpaceLeavesAPointUnchanged()
		{
			var window = new MockResolver().rendering.Window;
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(-0.5f, 0.6f),
				Zoom = 3.0f
			};

			Assert.AreEqual(new Point(1.2f, 3.4f),
				camera.ToPixelSpace(camera.FromPixelSpace(new Point(1.2f, 3.4f))));
			Assert.AreEqual(new Point(1.2f, 3.4f),
				camera.FromPixelSpace(camera.ToPixelSpace(new Point(1.2f, 3.4f))));
		}

		[Test]
		public void ToPixelSpace()
		{
			var window = new MockResolver().rendering.Window;
			var quadraticSize = new Size(window.ViewportPixelSize.Width);
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(-0.5f, 0.6f),
				Zoom = 2.0f
			};

			Assert.AreEqual(quadraticSize.Width * 1.5f, camera.ToPixelSpace(Point.Zero).X);
			Assert.AreEqual(new Point(1600, 52), camera.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Point(2240, 692), camera.ToPixelSpace(Point.One));
			Assert.AreEqual(quadraticSize, camera.ToPixelSpace(Size.Half));
		}

		[Test]
		public void FromPixelSpace()
		{
			var window = new MockResolver().rendering.Window;
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(-0.5f, 0.6f),
				Zoom = 2.0f
			};

			Assert.AreEqual(new Point(-0.75f, 0.459375f), camera.FromPixelSpace(Point.Zero));
			Assert.AreEqual(new Point(-0.25f, 0.740625f),
				camera.FromPixelSpace((Point)window.ViewportPixelSize));
			Assert.AreEqual(camera.LookAt, camera.FromPixelSpace((Point)window.ViewportPixelSize / 2));
		}

		[VisualTest]
		public void RenderPanAndZoomIntoLogo(Type resolver)
		{
			Camera2DControlledQuadraticScreenSpace camera = null;
			float elapsed = 0.0f;
			Start(resolver, (Camera2DControlledQuadraticScreenSpace cam, ContentLoader content) =>
			{
				camera = cam;
				new Sprite(content.Load<Image>("DeltaEngineLogo"),
					Rectangle.FromCenter(Point.One, new Size(0.25f)));
			}, () =>
			{
				elapsed += Time.Current.Delta;
				camera.LookAt = Point.Lerp(Point.Half, Point.One, elapsed / 2);
				camera.Zoom = MathExtensions.Lerp(1.0f, 2.0f, elapsed / 4);
			});
		}
	}
}