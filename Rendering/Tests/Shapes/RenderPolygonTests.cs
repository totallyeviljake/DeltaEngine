using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RenderPolygonTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderFilledEllipse(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f) { Color = Color.Blue };
				entitySystem.Add(ellipse.Add<RenderPolygon>());
			});
		}

		[IntegrationTest]
		public void RenderingPolygonWithNoPointsDoesNotError(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f) { Color = Color.Blue };
				var points = ellipse.Get<List<Point>>();
				points.Clear();
				ellipse.Remove<CalculateEllipsePoints>();
				entitySystem.Add(ellipse.Add<RenderPolygon>());
			});
		}
	}
}