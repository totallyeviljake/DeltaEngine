using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// Implements the Farseer flavor of physics bodies
	/// </summary>
	internal class FarseerBody : PhysicsBody
	{
		public FarseerBody(Body body)
		{
			Body = body;
			Body.BodyType = BodyType.Dynamic;
		}

		public Body Body { get; private set; }
		internal UnitConverter UnitConverter { get; set; }

		public Point Position
		{
			get
			{
				var position = UnitConverter.Convert(UnitConverter.ToDisplayUnits(Body.Position));
				return position;
			}
			set
			{
				Vector2 position = UnitConverter.Convert(value);
				Body.Position = UnitConverter.ToSimUnits(position);
			}
		}

		public bool IsStatic
		{
			get { return Body.IsStatic; }
			set { Body.IsStatic = value; }
		}

		public float Restitution
		{
			get { return Body.Restitution; }
			set { Body.Restitution = value; }
		}

		public float Friction
		{
			get { return Body.Friction; }
			set { Body.Friction = value; }
		}

		public float Rotation
		{
			get { return Body.Rotation.RadiansToDegrees(); }
			set { Body.Rotation = value.DegreesToRadians(); }
		}

		public Point LinearVelocity
		{
			get { return new Point(Body.LinearVelocity.X, Body.LinearVelocity.Y); }
			set { Body.LinearVelocity = new Vector2(value.X, value.Y); }
		}

		public void ApplyLinearImpulse(Point impulse)
		{
			Vector2 fImpulse = UnitConverter.Convert(impulse);
			Body.ApplyLinearImpulse(ref fImpulse);
		}

		public void ApplyAngularImpulse(float impulse)
		{
			Body.ApplyAngularImpulse(impulse);
		}

		public void ApplyTorque(float torque)
		{
			Body.ApplyTorque(torque);
		}

		public Point[] LineVertices
		{
			get
			{
				Transform xf;
				Body.GetTransform(out xf);
				var vertices = new List<Point>();
				foreach (var fixture in Body.FixtureList)
					vertices.AddRange(GetShapeVerticesFromFixture(fixture, xf));
				return vertices.ToArray();
			}
		}

		private IEnumerable<Point> GetShapeVerticesFromFixture(Fixture fixture, Transform xf)
		{
			var shape = fixture.Shape;
			switch (shape.ShapeType)
			{
			case ShapeType.Polygon:
				return GetPolygonShapeVertices(shape as PolygonShape, xf);
			case ShapeType.Edge:
				return GetEdgeShapeVertices(shape as EdgeShape, xf);
			case ShapeType.Circle:
				return GetCircleShapeVertices(shape as CircleShape, xf);
			}
			//This will never be reached, PhysicsBody internally will not allow it.
			return new List<Point>(); //ncrunch: no coverage
		}

		private IEnumerable<Point> GetPolygonShapeVertices(PolygonShape polygon, Transform xf)
		{
			var vertexCount = polygon.Vertices.Count;
			tempVertices = new Vector2[vertexCount];
			for (int i = 0; i < vertexCount; ++i)
				tempVertices[i] = MathUtils.Mul(ref xf, polygon.Vertices[i]);
			return GetDrawVertices(tempVertices, vertexCount);
		}

		private Vector2[] tempVertices;

		private IEnumerable<Point> GetDrawVertices(Vector2[] vertices, int vertexCount)
		{
			var drawVertices = new List<Point>();
			for (int i = 0; i < (vertexCount - 1); i++)
			{
				drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[i])));
				drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[i + 1])));
			}
			drawVertices.Add(
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[vertexCount - 1])));
			drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[0])));
			return drawVertices.ToArray();
		}

		private IEnumerable<Point> GetEdgeShapeVertices(EdgeShape edge, Transform xf)
		{
			var v1 = MathUtils.Mul(ref xf, edge.Vertex1);
			var v2 = MathUtils.Mul(ref xf, edge.Vertex2);

			return new[]
			{
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(v1)),
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(v2))
			};
		}

		private IEnumerable<Point> GetCircleShapeVertices(CircleShape circle, Transform xf)
		{
			CircleData circleData = CreateCircleData(circle, xf);
			return CreateCircleVertexArray(circleData);
		}

		private static CircleData CreateCircleData(CircleShape circle, Transform xf)
		{
			var circleData = new CircleData();
			circleData.center = MathUtils.Mul(ref xf, circle.Position);
			circleData.radius = circle.Radius;
			circleData.circleSegments = 32;
			circleData.increment = Math.PI * 2.0 / circleData.circleSegments;
			circleData.theta = 0.0;
			return circleData;
		}

		private struct CircleData
		{
			internal Vector2 center;
			internal float radius;
			internal int circleSegments;
			internal double increment;
			internal double theta;
		}

		private Point[] CreateCircleVertexArray(CircleData circleData)
		{
			var vertices = new List<Point>();
			for (int i = 0; i < circleData.circleSegments; i++)
			{
				Vector2 v1 = CreateCircleVertexVectorV1(circleData);
				Vector2 v2 = CreateCircleVertexVectorV2(circleData);

				vertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(v1)));
				vertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(v2)));

				circleData.theta += circleData.increment;
			}
			return vertices.ToArray();
		}

		private static Vector2 CreateCircleVertexVectorV1(CircleData circleData)
		{
			return circleData.center +
				circleData.radius *
					new Vector2((float)Math.Cos(circleData.theta), (float)Math.Sin(circleData.theta));
		}

		private static Vector2 CreateCircleVertexVectorV2(CircleData circleData)
		{
			return circleData.center +
				circleData.radius *
					new Vector2((float)Math.Cos(circleData.theta + circleData.increment),
						(float)Math.Sin(circleData.theta + circleData.increment));
		}

		public void Dispose()
		{
			Body.Dispose();
		}
	}
}