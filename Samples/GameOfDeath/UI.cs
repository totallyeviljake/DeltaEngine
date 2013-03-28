﻿using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// Displays the UI consisting of the surrounding screen borders, but also the grass background.
	/// </summary>
	public class UI : Sprite
	{
		public UI(Content content, Renderer renderer)
			: base(content.Load<Image>("GrassBackground"), renderer.Screen.Viewport)
		{
			RenderLayer = MinRenderLayer;
			AddUITop(content, renderer);
			AddUIBottom(content, renderer);
			AddUILeft(content, renderer);
			AddUIRight(content, renderer);
			renderer.Screen.ViewportSizeChanged += () => Update(renderer);
		}

		private void Update(Renderer renderer)
		{
			DrawArea = renderer.Screen.Viewport;
			topBorder.DrawArea = new Rectangle(DrawArea.Left, DrawArea.Top, DrawArea.Width, topHeight);
			bottomBorder.DrawArea = new Rectangle(DrawArea.Left, DrawArea.Bottom - bottomHeight,
				DrawArea.Width, bottomHeight);
			leftBorder.DrawArea = new Rectangle(DrawArea.Left, DrawArea.Top + topHeight, leftWidth,
				DrawArea.Height - (bottomHeight + topHeight));
			rightBorder.DrawArea = new Rectangle(DrawArea.Right - rightWidth, DrawArea.Top + topHeight,
				rightWidth, DrawArea.Height - (bottomHeight + topHeight));
		}

		private Sprite topBorder;
		private Sprite bottomBorder;
		private Sprite leftBorder;
		private Sprite rightBorder;
		private float topHeight;
		private float bottomHeight;
		private float leftWidth;
		private float rightWidth;

		private void AddUITop(Content content, Renderer renderer)
		{
			var topImage = content.Load<Image>("UITop");
			topHeight = topImage.PixelSize.Height / Scoreboard.QuadraticFullscreenSize.Height;
			topBorder = new Sprite(topImage,
				new Rectangle(DrawArea.Left, DrawArea.Top, DrawArea.Width, topHeight))
			{
				RenderLayer = ForegroundLayer
			};
			renderer.Add(topBorder);
		}

		private const byte ForegroundLayer = MaxRenderLayer - 2;

		private void AddUIBottom(Content content, Renderer renderer)
		{
			var bottomImage = content.Load<Image>("UIBottom");
			bottomHeight = bottomImage.PixelSize.Height / Scoreboard.QuadraticFullscreenSize.Height;
			bottomBorder = new Sprite(bottomImage,
				new Rectangle(DrawArea.Left, DrawArea.Bottom - bottomHeight, DrawArea.Width, bottomHeight))
			{
				RenderLayer = ForegroundLayer
			};
			renderer.Add(bottomBorder);
		}

		private void AddUILeft(Content content, Renderer renderer)
		{
			var leftImage = content.Load<Image>("UILeft");
			leftWidth = leftImage.PixelSize.Width / Scoreboard.QuadraticFullscreenSize.Width;
			leftBorder = new Sprite(leftImage,
				new Rectangle(DrawArea.Left, DrawArea.Top + topHeight, leftWidth,
					DrawArea.Height - (bottomHeight + topHeight))) { RenderLayer = ForegroundLayer };
			renderer.Add(leftBorder);
		}

		private void AddUIRight(Content content, Renderer renderer)
		{
			var rightImage = content.Load<Image>("UIRight");
			rightWidth = rightImage.PixelSize.Width / Scoreboard.QuadraticFullscreenSize.Width;
			rightBorder = new Sprite(rightImage,
				new Rectangle(DrawArea.Right - rightWidth, DrawArea.Top + topHeight, rightWidth,
					DrawArea.Height - (bottomHeight + topHeight))) { RenderLayer = ForegroundLayer };
			renderer.Add(rightBorder);
		}
	}
}