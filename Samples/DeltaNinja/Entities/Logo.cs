using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using System.Collections.Generic;

namespace DeltaNinja
{
	public class Logo : MovingSprite
	{
		public enum SizeCategory
		{
			Small = 5,
			Medium = 2,
			Big = 1
		}

		[System.Flags]
		private enum Sides
		{
			None = 0,
			Top = 1,
			Right = 2,
			Bottom = 4,
			Left = 8
		}

		public Logo(Image image, Color color, Point position, Size size, SimplePhysics.Data data)
			: base(image, color, position, size)
		{
			RenderLayer = (int)GameRenderLayer.Logos;

			sideStatus = Sides.None;

			Add(data);
			Add<TrajectoryEntityHandler>();
			Add<SimplePhysics.Rotate>();
		}

		private Sides sideStatus;

		public SizeCategory Category
		{
			get
			{
				float logoSize = DrawArea.Width;

				if (logoSize < 0.03f) return SizeCategory.Small;
				if (logoSize < 0.05f) return SizeCategory.Medium;
				return SizeCategory.Big;
			}
		}

		public void CheckForSlicing(Point start, Point end)
		{
			if (!sideStatus.HasFlag(Sides.Top) && CheckIfLineIntersectsLine(start, end, DrawArea.TopLeft, DrawArea.TopRight)) sideStatus |= Sides.Top;
			if (!sideStatus.HasFlag(Sides.Left) && CheckIfLineIntersectsLine(start, end, DrawArea.TopLeft, DrawArea.BottomLeft)) sideStatus |= Sides.Left;
			if (!sideStatus.HasFlag(Sides.Bottom) && CheckIfLineIntersectsLine(start, end, DrawArea.BottomLeft, DrawArea.BottomRight)) sideStatus |= Sides.Bottom;
			if (!sideStatus.HasFlag(Sides.Right) && CheckIfLineIntersectsLine(start, end, DrawArea.TopRight, DrawArea.BottomRight)) sideStatus |= Sides.Right;
		}

		public void ResetSlicing()
		{
			sideStatus = Sides.None;
		}

		public bool IsSliced
		{
			get
			{
				int count = 0;
				if (sideStatus.HasFlag(Sides.Top)) count++;
				if (sideStatus.HasFlag(Sides.Right)) count++;
				if (sideStatus.HasFlag(Sides.Bottom)) count++;
				if (sideStatus.HasFlag(Sides.Left)) count++;
				return count > 1;
			}
		}

		private bool IsSideSliced(List<Line2D> segments, Point startPoint, Point endPoint)
		{
			foreach (var segment in segments)
			{
				if (CheckIfLineIntersectsLine(segment.Start, segment.End, startPoint, endPoint)) return true;
			}
			return false;
		}

	}
}