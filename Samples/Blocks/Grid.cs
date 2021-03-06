using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace Blocks
{
	/// <summary>
	/// Represents a grid of bricks: the blocks that have come to rest and not yet been removed
	/// </summary>
	public class Grid
	{
		public Grid(BlocksContent content)
		{
			this.content = content;
		}

		private readonly BlocksContent content;

		public int AffixBlock(Block block)
		{
			foreach (Brick brick in block.Bricks.Where(brick => !IsOccupied(brick)))
				AffixBrick(brick);

			RemoveFilledRows();
			return removedRows;
		}

		private bool IsOccupied(Brick brick)
		{
			return bricks[(int)brick.Position.X, (int)brick.Position.Y] != null;
		}

		internal readonly Brick[,] bricks = new Brick[Width,Height];

		private void AffixBrick(Brick brick)
		{
			brick.TopLeftGridCoord = new Point((int)brick.TopLeftGridCoord.X,
				(int)brick.TopLeftGridCoord.Y + 1);
			bricks[(int)brick.Position.X, (int)brick.Position.Y - 1] = brick;
			brick.UpdateDrawArea();
			AddZoomingBrick(brick);
		}

		internal const int Width = 12;
		internal const int Height = 19;

		private void RemoveFilledRows()
		{
			removedRows = 0;
			for (int y = 0; y < Height; y++)
				if (IsRowFilled(y))
					RemoveRow(y);
		}

		private int removedRows;

		private bool IsRowFilled(int y)
		{
			for (int x = 0; x < Width; x++)
				if (bricks[x, y] == null)
					return false;

			return true;
		}

		private void RemoveRow(int row)
		{
			for (int x = 0; x < Width; x++)
				RemoveBrick(x, row);

			for (int x = 0; x < Width; x++)
				for (int y = row; y > 0; y--)
					MoveBrickDown(x, y);

			removedRows++;
		}

		private void RemoveBrick(int x, int y)
		{
			var brick = bricks[x, y];
			brick.IsActive = false;
			bricks[x, y] = null;
			if (content.DoBricksSplitInHalfWhenRowFull)
				AddPairOfFallingBricks(brick);
			else
				AddFallingBrick(brick, brick.Image);
		}

		private void AddPairOfFallingBricks(Brick brick)
		{
			AddTopFallingBrick(brick);
			AddBottomFallingBrick(brick);
		}

		private void AddTopFallingBrick(Sprite brick)
		{
			var filename = content.GetFilenameWithoutPrefix(brick.Image.Name);
			var image = content.Load<Image>(filename + "_Top");
			AddFallingBrick(brick, image);
		}

		private void AddBottomFallingBrick(Sprite brick)
		{
			var filename = content.GetFilenameWithoutPrefix(brick.Image.Name);
			var image = content.Load<Image>(filename + "_Bottom");
			AddFallingBrick(brick, image);
		}

		private static void AddFallingBrick(Entity2D brick, Image image)
		{
			var fallingBrick = new Sprite(image, brick.DrawArea)
			{
				Color = brick.Color,
				RenderLayer = (int)RenderLayer.FallingBrick,
			};
			var random = Randomizer.Current;
			fallingBrick.Add(new SimplePhysics.Data
			{
				Velocity = new Point(random.Get(-0.5f, 0.5f), random.Get(-1.0f, 0.0f)),
				RotationSpeed = random.Get(-360, 360),
				Duration = 5.0f,
				Gravity = new Point(0.0f, 2.0f)
			});
			fallingBrick.Start<SimplePhysics.Fall>();
		}

		private void MoveBrickDown(int x, int y)
		{
			bricks[x, y] = bricks[x, y - 1];
			if (bricks[x, y] == null)
				return;

			bricks[x, y].TopLeftGridCoord.Y++;
			bricks[x, y].UpdateDrawArea();
		}

		private static void AddZoomingBrick(Sprite brick)
		{
			var zoom = new Sprite(brick.Image, brick.DrawArea)
			{
				Color = brick.Color,
				RenderLayer = (int)RenderLayer.ZoomingBrick,
			};
			zoom.Add(new Transition.Duration(0.2f));
			zoom.Add(new Transition.Size(brick.DrawArea.Size, 2 * brick.DrawArea.Size));
			zoom.Add(new Transition.FadingColor(brick.Color));
			zoom.Start<FinalTransition>();
		}

		public bool IsValidPosition(Block block)
		{
			foreach (Brick brick in block.Bricks)
				if (IsOutsideTheGrid(brick) || IsOccupied(brick))
					return false;

			return true;
		}

		private static bool IsOutsideTheGrid(Brick brick)
		{
			return brick.Position.X < 0 || brick.Position.X >= Width || brick.Position.Y < 1 ||
				brick.Position.Y >= Height;
		}

		public List<int> GetValidStartingColumns(Block block)
		{
			block.Top = 1;
			List<int> validStartingColumns = content.DoBlocksStartInARandomColumn
				? GetAllValidStartingColumns(block) : GetMiddleColumnIfValid(block);

			return validStartingColumns;
		}

		private List<int> GetAllValidStartingColumns(Block block)
		{
			var validStartingColumns = new List<int>();
			for (int x = 0; x < Width; x++)
				if (IsAValidStartingColumn(block, x))
					validStartingColumns.Add(x);

			return validStartingColumns;
		}

		private bool IsAValidStartingColumn(Block block, int column)
		{
			block.Left = column;
			return IsValidPosition(block);
		}

		private List<int> GetMiddleColumnIfValid(Block block)
		{
			var validStartingColumns = new List<int>();
			if (IsAValidStartingColumn(block, Middle))
				validStartingColumns.Add(Middle - (int)block.Center.X);

			return validStartingColumns;
		}

		private const int Middle = Width / 2;

		public bool IsABrickOnFirstRow()
		{
			for (int x = 0; x < Width; x++)
				if (bricks[x, 0] != null)
					return true;

			return false;
		}

		public void Clear()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					if (bricks[x, y] != null)
						RemoveBrick(x, y);
		}
	}
}