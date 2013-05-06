using DeltaEngine.Datatypes;

namespace Blobs.Creatures
{
	/// <summary>
	/// Controls the right eyebrow of a Blob
	/// </summary>
	public class RightEyebrow : Eyebrow
	{
		public RightEyebrow(Eye eye, Mood mood)
			: base(eye, mood) {}

		public override void Run()
		{
			Start = eye.Center -
				new Point(eye.RadiusX, (2.0f - (0.5f * mood.Anger) - (0.25f * mood.Anger)) * eye.RadiusY);
			End = eye.Center +
				new Point(eye.RadiusX, (-1.5f - (0.5f * mood.Anger) + (0.25f * mood.Anger)) * eye.RadiusY);

			base.Run();
		}
	}
}