using SharpDX.Direct3D11;

namespace DeltaEngine.Graphics.SharpDX
{
	public struct CircularBuffer
	{
		public CircularBuffer(int numberOfChunks)
		{
			Handle = null;
			NumberOfChunks = numberOfChunks;
			CurrentChunk = 0;
		}

		public Buffer Handle;
		public int NumberOfChunks;
		public int CurrentChunk;

		public void SelectNextChunk()
		{
			CurrentChunk++;
			if (CurrentChunk == NumberOfChunks)
				CurrentChunk = 0;
		}
	}
}