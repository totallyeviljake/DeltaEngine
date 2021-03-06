using DeltaEngine.Datatypes;
using DeltaEngine.Input.Windows;
using DInput = SlimDX.DirectInput;

namespace DeltaEngine.Input.SlimDX
{
	/// <summary>
	/// Native implementation of the Mouse interface using DirectInput
	/// </summary>
	public class SlimDXMouse : Mouse
	{
		public SlimDXMouse(CursorPositionTranslater positionTranslater)
		{
			this.positionTranslater = positionTranslater;
			mouseCounter = new MouseDeviceCounter();
			directInput = new DInput.DirectInput();
			mouse = new DInput.Mouse(directInput);
			mouse.Properties.AxisMode = DInput.DeviceAxisMode.Absolute;
			mouse.Acquire();
			currentState = new DInput.MouseState();
		}

		private readonly CursorPositionTranslater positionTranslater;
		private readonly MouseDeviceCounter mouseCounter;
		private DInput.DirectInput directInput;
		private DInput.Mouse mouse;
		private DInput.MouseState currentState;

		public override bool IsAvailable
		{
			get { return mouseCounter.GetNumberOfAvailableMice() > 0; }
		}

		public override void SetPosition(Point newPosition)
		{
			positionTranslater.SetCursorPosition(newPosition);
		}

		public override void Dispose()
		{
			if (mouse != null)
				mouse.Unacquire();
			mouse = null;
			directInput = null;
		}

		public override void Run()
		{
			mouse.GetCurrentState(ref currentState);
			ScrollWheelValue = currentState.Z;
			UpdateMousePosition();
			UpdateMouseButtons();
		}

		private void UpdateMousePosition()
		{
			Position = positionTranslater.GetCursorPosition();
		}

		private void UpdateMouseButtons()
		{
			LeftButton = LeftButton.UpdateOnNativePressing(currentState.GetButtons()[0]);
			RightButton = RightButton.UpdateOnNativePressing(currentState.GetButtons()[1]);
			MiddleButton = MiddleButton.UpdateOnNativePressing(currentState.GetButtons()[2]);
			X1Button = X1Button.UpdateOnNativePressing(currentState.GetButtons()[3]);
			X2Button = X2Button.UpdateOnNativePressing(currentState.GetButtons()[4]);
		}
	}
}