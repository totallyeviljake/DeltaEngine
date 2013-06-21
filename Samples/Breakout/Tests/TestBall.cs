using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace Breakout.Tests
{
	public class TestBall : Ball
	{
		public TestBall(Paddle paddle, ContentLoader content, InputCommands inputCommands)
			: base(paddle, content, inputCommands) {}

		public Point CurrentVelocity
		{
			get { return velocity; }
			set { velocity = value; }
		}

		public bool IsCurrentlyOnPaddle
		{
			get { return isOnPaddle; }
		}

		public void SetPosition(Point newPosition)
		{
			isOnPaddle = false;
			Position = newPosition;
		}
	}
}