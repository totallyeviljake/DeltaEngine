using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath
{
	/// <summary>
	/// Displays the UI consisting of the surrounding screen borders, but also the grass background.
	/// </summary>
	public class UI
	{
		public UI(ScreenSpace screen, SoundDevice soundDevice)
		{
			background = new Sprite(ContentLoader.Load<Image>("GrassBackground"), screen.Viewport)
			{
				RenderLayer = (int)GameCoordinator.RenderLayers.Background
			};
			screen.Window.WindowClosing += soundDevice.Dispose;
			AddUITop();
			AddUIBottom();
			AddUILeft();
			AddUIRight();
			screen.ViewportSizeChanged += () => Update(screen);
			Update(screen);
		}

		private readonly Sprite background;

		private void AddUITop()
		{
			var topImage = ContentLoader.Load<Image>("UITop");
			topHeight = topImage.PixelSize.Height / Scoreboard.QuadraticFullscreenSize.Height;
			topBorder = new Sprite(topImage,
				new Rectangle(viewport.Left, viewport.Top, viewport.Width, topHeight))
			{
				RenderLayer = (int)GameCoordinator.RenderLayers.Gui
			};
		}

		private float topHeight;
		private Sprite topBorder;
		private Rectangle viewport;

		private void AddUIBottom()
		{
			var bottomImage = ContentLoader.Load<Image>("UIBottom");
			bottomHeight = bottomImage.PixelSize.Height / Scoreboard.QuadraticFullscreenSize.Height;
			bottomBorder = new Sprite(bottomImage,
				new Rectangle(viewport.Left, viewport.Bottom - bottomHeight, viewport.Width, bottomHeight))
			{
				RenderLayer = (int)GameCoordinator.RenderLayers.Gui
			};
		}

		private float bottomHeight;
		private Sprite bottomBorder;

		private void AddUILeft()
		{
			var leftImage = ContentLoader.Load<Image>("UILeft");
			leftWidth = leftImage.PixelSize.Width / Scoreboard.QuadraticFullscreenSize.Width;
			leftBorder = new Sprite(leftImage,
				new Rectangle(viewport.Left, viewport.Top + topHeight, leftWidth,
					viewport.Height - (bottomHeight + topHeight)))
			{
				RenderLayer = (int)GameCoordinator.RenderLayers.Gui
			};
		}

		private float leftWidth;
		private Sprite leftBorder;

		private void AddUIRight()
		{
			var rightImage = ContentLoader.Load<Image>("UIRight");
			rightWidth = rightImage.PixelSize.Width / Scoreboard.QuadraticFullscreenSize.Width;
			rightBorder = new Sprite(rightImage,
				new Rectangle(viewport.Right - rightWidth, viewport.Top + topHeight, rightWidth,
					viewport.Height - (bottomHeight + topHeight)))
			{
				RenderLayer = (int)GameCoordinator.RenderLayers.Gui
			};
		}

		private float rightWidth;
		private Sprite rightBorder;

		private void Update(ScreenSpace screen)
		{
			viewport = screen.Viewport;
			background.DrawArea = viewport;
			topBorder.DrawArea = new Rectangle(viewport.Left, viewport.Top, viewport.Width, topHeight);
			bottomBorder.DrawArea = new Rectangle(viewport.Left, viewport.Bottom - bottomHeight,
				viewport.Width, bottomHeight);
			leftBorder.DrawArea = new Rectangle(viewport.Left, viewport.Top + topHeight, leftWidth,
				viewport.Height - (bottomHeight + topHeight));
			rightBorder.DrawArea = new Rectangle(viewport.Right - rightWidth, viewport.Top + topHeight,
				rightWidth, viewport.Height - (bottomHeight + topHeight));
		}
	}
}