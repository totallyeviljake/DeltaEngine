using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class Line2DTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderManyRotatingLines(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				for (int num = 0; num < 100; num++)
					AddRotatingLine(num, entitySystem);
			});
		}

		private static void AddRotatingLine(int num, EntitySystem entitySystem)
		{
			var line = new Line2D(Point.Half, new Point(0.5f, 0.0f), Color.Orange);
			line.Rotation = num * 360 / 100.0f;
			line.Add<Rotate>();
			entitySystem.Add(line);
		}

		public class Rotate : EntityHandler
		{
			public void Handle(List<Entity> entities)
			{
				foreach (var entity in entities.OfType<Entity2D>())
					RotateLine(entity);
			}

			private static void RotateLine(Entity2D entity)
			{
				entity.Rotation += 10 * Time.Current.Delta;
				var line = entity as Line2D;
				if (line != null)
					line.End = Point.Half + new Point(0.5f, 0).RotateAround(Point.Zero, entity.Rotation);
			}

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[Test]
		public void CreateLine()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red);
			Assert.AreEqual(Point.Zero, line.Start);
			Assert.AreEqual(Point.One, line.End);
			Assert.AreEqual(Color.Red, line.Color);
			Assert.AreEqual(2, line.Points.Count);
		}

		[Test]
		public void MoveLineViaItsEndPoints()
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red)
			{
				Start = Point.Half,
				End = Point.One
			};
			Assert.AreEqual(Point.Half, line.Start);
			Assert.AreEqual(Point.One, line.End);
		}

		[Test]
		public void MoveLineByAssigningListOfPoints()
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red);
			line.Points = new List<Point> { Point.Half, Point.One };
			Assert.AreEqual(Point.Half, line.Start);
			Assert.AreEqual(Point.One, line.End);
		}

		[Test]
		public void AddLineToEntitySystem()
		{
			var resolver = new MockResolver();
			var line = new Line2D(Point.Zero, Point.One, Color.Red);
			resolver.EntitySystem.Add(line);
			resolver.EntitySystem.Run();
			Assert.AreEqual(2, resolver.rendering.NumberOfVerticesDrawn);
		}

		[Test]
		public void ChangeColor()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red) { Color = Color.Green };
			Assert.AreEqual(Color.Green, line.Color);
			Assert.AreEqual(Color.Green, line.Get<Color>());
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void RenderRedLine(Type resolver)
		{
			Start(resolver,
				(EntitySystem entitySystem) =>
					entitySystem.Add(new Line2D(Point.UnitX, Point.UnitY, Color.Red)));
		}

		[VisualTest]
		public void RenderRedSquareViaAddingLines(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.4f), Color.Red);
				line.AddLine(new Point(0.6f, 0.4f), new Point(0.6f, 0.6f));
				line.AddLine(new Point(0.6f, 0.6f), new Point(0.4f, 0.6f));
				line.AddLine(new Point(0.4f, 0.6f), new Point(0.4f, 0.4f));
				entitySystem.Add(line);
			});
		}

		[VisualTest]
		public void RenderRedSquareViaExtendingLine(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) => entitySystem.Add(CreateRedBox()));
		}

		private static Line2D CreateRedBox()
		{
			var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.4f), Color.Red);
			line.ExtendLine(new Point(0.6f, 0.6f));
			line.ExtendLine(new Point(0.4f, 0.6f));
			line.ExtendLine(new Point(0.4f, 0.4f));
			return line;
		}

		[VisualTest]
		public void RenderRedSquareWithMissingTop(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var line = CreateRedBox();
				var points = line.Points;
				points.RemoveAt(0);
				points.RemoveAt(0);
				entitySystem.Add(line);
			});
		}
	}
}