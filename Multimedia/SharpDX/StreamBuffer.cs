﻿using System;
using SharpDX.XAudio2;
using ToyMp3;

namespace DeltaEngine.Multimedia.SharpDX
{
	/// <summary>
	/// Helper class wrapping an XAudio buffer object and streaming the next data into the buffer.
	/// </summary>
	internal class StreamBuffer : IDisposable
	{
		public StreamBuffer()
		{
			XAudioBuffer = new AudioBuffer();
			byteBuffer = new byte[BufferSize];
		}

		public AudioBuffer XAudioBuffer { get; private set; }
		private byte[] byteBuffer;
		private const int BufferSize = 4096 * 8;

		public bool FillFromStream(Mp3Stream stream)
		{
			if (stream == null)
				return false;

			int size = stream.Read(byteBuffer, BufferSize);
			bool dataAvailable = size > 0;
			if (dataAvailable)
			{
				XAudioBuffer.AudioDataPointer = GetBufferHandle();
				XAudioBuffer.AudioBytes = size;
				int blockAlign = stream.Channels * 2;
				XAudioBuffer.PlayLength = size / blockAlign;
			}

			return dataAvailable;
		}

		private unsafe IntPtr GetBufferHandle()
		{
			fixed (byte* ptr = &byteBuffer[0])
				return (IntPtr)ptr;
		}

		public void Dispose()
		{
			XAudioBuffer = null;
			byteBuffer = null;
		}
	}
}