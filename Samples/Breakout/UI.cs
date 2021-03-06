using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;

namespace Breakout
{
	/// <summary>
	/// Primes the window to respond to keyboard commands and launches the game
	/// </summary>
	public class UI : Runner
	{
		public UI(Window window, InputCommands inputCommands, Game game, SoundDevice soundDevice)
		{
			this.window = window;
			this.window.WindowClosing += ()=>soundDevice.Dispose();
			inputCommands.Add(Key.Escape, key=>window.Dispose());
			inputCommands.Add(Key.F, key => window.SetFullscreen(new Size(1920, 1080)));
			this.game = game;
		}

		private readonly Window window;
		private readonly Game game;

		public void Run()
		{
			if (Time.Current.CheckEvery(0.2f))
				window.Title = "Breakout " + game.Score;
		}
	}
}