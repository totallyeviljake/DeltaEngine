using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	internal class BodyTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateAndSetValues()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				physics.Gravity = Point.UnitY;
				Assert.AreEqual(Point.UnitY, physics.Gravity);
				var physicsBody = physics.CreateRectangle(new Size(0.5f));
				SetAndCheckLinearVelocity(physicsBody);
				SetAndCheckRotation(physicsBody);
				SetAndCheckPosition(physicsBody);
				SetAndCheckStatic(physicsBody);
				SetAndCheckRestitution(physicsBody);
				SetAndCheckFriction(physicsBody);
			});
		}

		private static void SetAndCheckStatic(PhysicsBody physicsBody)
		{
			physicsBody.IsStatic = true;
			Assert.IsTrue(physicsBody.IsStatic);
		}

		private static void SetAndCheckRotation(PhysicsBody physicsBody)
		{
			physicsBody.Rotation = 30;
			Assert.AreEqual(30, physicsBody.Rotation);
		}

		private static void SetAndCheckPosition(PhysicsBody physicsBody)
		{
			physicsBody.Position = Point.Half;
			Assert.AreEqual(Point.Half, physicsBody.Position);
		}

		private static void SetAndCheckRestitution(PhysicsBody physicsBody)
		{
			physicsBody.Restitution = 1;
			Assert.AreEqual(1, physicsBody.Restitution);
		}

		private static void SetAndCheckFriction(PhysicsBody physicsBody)
		{
			physicsBody.Friction = 0.5f;
			Assert.AreEqual(0.5f, physicsBody.Friction);
		}

		private static void SetAndCheckLinearVelocity(PhysicsBody physicsBody)
		{
			physicsBody.LinearVelocity = new Point(5, -5);
			Assert.AreEqual(new Point(5, -5), physicsBody.LinearVelocity);
		}

		[Test]
		public void TestApplyLinearImpulse()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
				Point originalPosition = body.Position;
				body.ApplyLinearImpulse(Point.Zero);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.AreNotEqual(originalPosition, body.Position);
			});
		}

		[Test]
		public void TestApplyAngularImpulse()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
				float originalRotation = body.Rotation;
				body.ApplyAngularImpulse(10.0f);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.AreNotEqual(originalRotation, body.Rotation);
			});
		}

		[Test]
		public void TestApplyTorque()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
				float originalRotation = body.Rotation;
				body.ApplyTorque(10.0f);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.AreNotEqual(originalRotation, body.Rotation);
			});
		}

		[Test]
		public void CreateEdgeFromTwoSinglePoints()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				Point[] edgePoints = { Point.Zero, Point.Half };
				var edge = physics.CreateEdge(Point.Zero, Point.Half);
				Assert.AreEqual(edgePoints, edge.LineVertices);
			});
		}

		[Test]
		public void CreateEdgeFromPointArray()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				Point[] edgePoints = { Point.Zero, Point.Half };
				var edge = physics.CreateEdge(edgePoints);
				Assert.AreEqual(edgePoints, edge.LineVertices);
			});
		}

		[Test]
		public void CreateCircleAndGetVertices()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateCircle(0.5f);
				Assert.IsNotEmpty(body.LineVertices);
			});
		}

		[Test]
		public void CreatePolygonFromPoints()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				Point[] polyPoints = { Point.Zero, Point.Half, Point.One, Point.UnitX };
				var body = physics.CreatePolygon(polyPoints);
				Assert.IsNotEmpty(body.LineVertices);
			});
		}

		[Test]
		public void DisposeBody()
		{
			Start(typeof(MockResolver), (Physics physics) =>
			{
				var body = physics.CreateRectangle(new Size(1));
				body.Dispose();
			});
		}
	}
}