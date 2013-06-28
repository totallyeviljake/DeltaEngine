using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace FindTheWord
{
	public class StartupScreen : Sprite
	{
		public StartupScreen(InputCommands input)
			: base(ContentLoader.Load<Image>("Startscreen"), GetDrawArea())
		{
			Input = input;
			CreateStartButton();
		}

		private static Rectangle GetDrawArea()
		{
			return new Rectangle(0, 0.1875f, 1, 0.625f);
		} 

		private InputCommands Input { get; set; }

		private void CreateStartButton()
		{
			const float ButtonWidth = 268.0f / 1280.0f;
			const float ButtonHeight = 164.0f / 1280.0f;
			const float BottomRightGap = 0.025f;
			float xPos = DrawArea.Right - BottomRightGap - ButtonWidth;
			float yPos = DrawArea.Bottom - BottomRightGap - ButtonHeight;
			Rectangle startDrawArea = new Rectangle(xPos, yPos, ButtonWidth, ButtonHeight);
			StartButton = new Button(Input, "StartButton", startDrawArea);
			StartButton.Clicked += button => StartGame();
		}

		public Button StartButton { get; private set; }

		public void StartGame()
		{
			if (GameStarted != null)
				GameStarted();
			FadeOut();
		}

		public void FadeOut()
		{
			Visibility = Visibility.Hide;
			StartButton.Visibility = Visibility.Hide;
		}

		public event Action GameStarted;
	}
}