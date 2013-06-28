using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja.Entities
{
	public class MovingSprite : Sprite
	{
		private static int ID = 0;
		public const float Gravity = 0.5f; 
		
		public MovingSprite(Image image, Color color, Point position, Size size)
			: base(image, Rectangle.One)
		{
			Size = size;
			Center = position;

			ID += 1;
		}

		public bool IsPaused { get; protected set; }

		public virtual void SetPause(bool pause)
		{
			IsPaused = pause;
		}

		private bool CheckBounds(Rectangle view)
		{
			if (view.Left > DrawArea.Left) return false;
			if (view.Right < DrawArea.Right) return false;
			if (DrawArea.Top < 0) return false;
			if (view.Top + 0.05f > DrawArea.Center.Y) return false;
			return true;
		}

		protected static bool CheckIfLineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
		{
			// See http://stackoverflow.com/questions/5514366/how-to-know-if-a-line-intersects-a-rectangle

			float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
			float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

			if (d == 0)
			{
				return false;
			}

			float r = q / d;

			q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
			float s = q / d;

			if (r < 0 || r > 1 || s < 0 || s > 1)
			{
				return false;
			}

			return true;
		}

		public bool IsOutside(Rectangle view)
		{
			return (DrawArea.Top > view.Bottom);
		}
	}
}
