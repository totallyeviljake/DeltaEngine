using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Logging;
using DeltaEngine.Multimedia.GLFW.Helpers;
using System.Diagnostics;
using DeltaEngine.Platforms;

namespace DeltaEngine.Multimedia.GLFW
{
	public class GLFWMusic : Music
	{
		public GLFWMusic(string filename, GLFWSoundDevice device, Settings settings) : base(filename, 
			device, settings)
		{
			openAL = device;
			channelHandle = openAL.CreateChannel();
			buffers = openAL.CreateBuffers(NumberOfBuffers);
			bufferData = new byte[BufferSize];
		}

		private GLFWSoundDevice openAL;
		private int channelHandle;
		private int[] buffers;
		private byte[] bufferData;
		private const int NumberOfBuffers = 2;
		private const int BufferSize = 1024 * 8;
		private BaseMusicStream musicStream;
		private AudioFormat format;
		private DateTime playStartTime;

		public override float DurationInSeconds
		{
			get
			{
				return musicStream.LengthInSeconds;
			}
		}

		public override float PositionInSeconds
		{
			get
			{
				var seconds = (float)DateTime.Now.Subtract(playStartTime).TotalSeconds;
				return MathExtensions.Round(seconds.Clamp(0f, DurationInSeconds), 2);
			}
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				var stream = new MemoryStream();
				fileData.CopyTo(stream);
				stream.Seek(0, SeekOrigin.Begin);
				if (IsOggStream(stream))
					musicStream = new OggMusicStream(stream);
				else
					musicStream = new Mp3MusicStream(stream);
				format = musicStream.Channels == 2 ? AudioFormat.Stereo16 : AudioFormat.Mono16;
			}
			catch (Exception ex)
			{
				Logger.Current.Error(ex);
				if (Debugger.IsAttached)
					throw new MusicNotFoundOrAccessible(Name, ex);
			}
		}

		private bool IsOggStream(MemoryStream stream)
		{
			byte[] magicBytes = new byte[3];
			stream.Read(magicBytes, 0, magicBytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return magicBytes [0] == 'O' && magicBytes [1] == 'g' && magicBytes [2] == 'g';
		}

		protected override void PlayNativeMusic(float volume)
		{
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream(buffers [index]))
					break;

			openAL.Play(channelHandle);
			openAL.SetVolume(channelHandle, volume);
			playStartTime = DateTime.Now;
		}

		protected override void StopNativeMusic()
		{
			openAL.Stop(channelHandle);
			EmptyBuffers();
		}

		private void EmptyBuffers()
		{
			int queued = openAL.GetNumberOfBuffersQueued(channelHandle);
			while (queued-- > 0)
				openAL.UnqueueBufferFromChannel(channelHandle);
		}

		public override bool IsPlaying()
		{
			return GetState() != ChannelState.Stopped;
		}

		protected override void Run()
		{
			if (UpdateBuffersAndCheckFinished())
				Stop();
			else if (!IsPlaying())
				openAL.Play(channelHandle);
		}

		private ChannelState GetState()
		{
			return openAL.GetChannelState(channelHandle);
		}

		private bool UpdateBuffersAndCheckFinished()
		{
			int processed = openAL.GetNumberOfBuffersProcesed(channelHandle);
			while (processed-- > 0)
			{
				int buffer = openAL.UnqueueBufferFromChannel(channelHandle);
				if (!Stream(buffer))
					return true;
			}
			return false;
		}

		private bool Stream(int buffer)
		{
			try
			{
				int bytesRead = musicStream.Read(bufferData, BufferSize);
				if (bytesRead == 0)
					return false;

				openAL.BufferData(buffer, format, bufferData, bytesRead, musicStream.Samplerate);
				openAL.QueueBufferInChannel(buffer, channelHandle);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			openAL.DeleteBuffers(buffers);
			openAL.DeleteChannel(channelHandle);
			musicStream = null;
		}
	}
}