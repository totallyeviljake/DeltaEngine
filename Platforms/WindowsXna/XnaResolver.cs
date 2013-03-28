﻿using System;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Input.Windows;
using DeltaEngine.Input.Xna;
using DeltaEngine.Multimedia.Xna;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class XnaResolver : WindowsResolver
	{
		public XnaResolver()
		{
			game = new XnaGame(this, RaiseInitializedEvent);
			RegisterInstance(game);
			RegisterSingleton<XnaWindow>();
			RegisterSingleton<XnaSoundDevice>();
			Register<XnaImage>();
			RegisterSingleton<XnaDevice>();
			Register<XnaSound>();
			Register<XnaMusic>();
			Register<XnaVideo>();
			RegisterSingleton<XnaDrawing>();
			RegisterSingleton<XnaMouse>();
			RegisterSingleton<XnaKeyboard>();
			RegisterSingleton<XnaTouch>();
			RegisterSingleton<CursorPositionTranslater>();
			RegisterSingleton<XnaGamePad>();
		}

		private readonly XnaGame game;

		/// <summary>
		/// Instead of starting the game normally and blocking we will delay the initialization in
		/// XnaGame until the game class has been constructed and the graphics device is available.
		/// </summary>
		public override void Run(Action runCode = null)
		{
			Resolve<XnaDevice>();
			game.Run(runCode);
		}
	}
}