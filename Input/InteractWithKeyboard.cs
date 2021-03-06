using System;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows 2D entities to respond to keyboard input by sending them a message on a key press or hold
	/// </summary>
	public class InteractWithKeyboard : Behavior2D
	{
		public InteractWithKeyboard(InputCommands input)
		{
			foreach (Key key in Enum.GetValues(typeof(Key)))
				AddEventsForKey(input, key);
		}

		private void AddEventsForKey(InputCommands input, Key key)
		{
			input.Add(key, PressKey);
			input.Add(key, State.Pressed, HoldKey);
		}

		private void PressKey(Key key)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			foreach (var entity in entities)
				entity.MessageAllListeners(new KeyPress(key));
		}

		public class KeyPress
		{
			public KeyPress(Key key)
			{
				Key = key;
			}

			public Key Key { get; private set; }
		}

		private void HoldKey(Key key)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			foreach (var entity in entities)
				entity.MessageAllListeners(new KeyHeld(key));
		}

		public class KeyHeld
		{
			public KeyHeld(Key key)
			{
				Key = key;
			}

			public Key Key { get; private set; }
		}

		public override void Handle(Entity2D entity) {}
	}
}