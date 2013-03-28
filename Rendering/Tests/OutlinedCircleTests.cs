﻿using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering.Tests
{
	public class OutlinedCircleTests : TestStarter
	{
		[Test]
		public void ConstructorWithColor()
		{
			var circle = new OutlinedCircle(Point.Half, 0.1f, Color.Blue);
			Assert.AreEqual(Point.Half, circle.Center);
			Assert.AreEqual(0.1f, circle.Radius);
			Assert.AreEqual(Color.Blue, circle.Color);
		}

		[Test]
		public void ConstructorWithoutColor()
		{
			var circle = new OutlinedCircle(Point.Half, 0.1f);
			Assert.AreEqual(Point.Half, circle.Center);
			Assert.AreEqual(0.1f, circle.Radius);
		}

		[Test]
		public void Center()
		{
			var circle = new OutlinedCircle(Point.Half, 0.1f) { Center = Point.One };
			Assert.AreEqual(Point.One, circle.Center);
			circle.Radius = 0.1f;
			Assert.AreEqual(0.1f, circle.Radius);
		}

		[Test]
		public void Radius()
		{
			var circle = new OutlinedCircle(Point.Half, 0.1f) { Radius = 0.2f, };
			Assert.AreEqual(0.2f, circle.Radius);
		}

		[Test]
		public void ColorProperty()
		{
			var circle = new OutlinedCircle(Point.Half, 0.1f) { Color = Color.Green };
			Assert.AreEqual(Color.Green, circle.Color);
		}

		[VisualTest]
		public void DrawLotsOfCircles(Type resolver)
		{
			Start(resolver, (Renderer renderer, Randomizer random) =>
			{
				for (int i = 0; i < 10; i++)
					renderer.Add(new OutlinedCircle(new Point(random.Get(), random.Get()), 0.0f,
						Color.GetRandomColor()) { Radius = random.Get(0.02f, 0.4f) });
			});
		}
	}
}