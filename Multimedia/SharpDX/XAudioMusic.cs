using System;
using System.IO;
using DeltaEngine.Core;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System.Diagnostics;
using ToyMp3;

namespace DeltaEngine.Multimedia.SharpDX
{
	/// <summary>
	/// Native XAudio implementation for music playback.
	/// </summary>
	public class XAudioMusic : Music
	{
		public XAudioMusic(string filename, XAudioDevice device)
			: base(filename, device)
		{
			CreateBuffers();
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				var stream = new MemoryStream();
				fileData.CopyTo(stream);
				musicStream = new Mp3Stream(stream);
				source = new SourceVoice((device as XAudioDevice).XAudio2,
					new WaveFormat(musicStream.Samplerate, 16, musicStream.Channels), false);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached)
					throw new MusicNotFoundOrAccessible(Name, ex);
			}
		}

		private Mp3Stream musicStream;
		private SourceVoice source;

		private void CreateBuffers()
		{
			buffers = new StreamBuffer[NumberOfBuffers];
			for (int i = 0; i < NumberOfBuffers; i++)
				buffers[i] = new StreamBuffer();
		}

		private StreamBuffer[] buffers;
		private const int NumberOfBuffers = 2;

		protected override void PlayNativeMusic(float volume)
		{
			if (isPlaying)
				return;

			source.Start();
			isPlaying = true;
			playStartTime = DateTime.Now;
		}

		private DateTime playStartTime;
		private bool isPlaying;

		protected override void Run()
		{
			while (source.State.BuffersQueued < NumberOfBuffers)
			{
				PutInStreamIfDataAvailable();
				if (isAbleToStream)
					continue;
				Stop();
				break;
			}
		}

		protected override void StopNativeMusic()
		{
			if (!isPlaying)
				return;

			isPlaying = false;
			source.Stop();
			source.FlushSourceBuffers();
		}
		
		public override bool IsPlaying()
		{
			return isPlaying;
		}

		private void PutInStreamIfDataAvailable()
		{
			StreamBuffer currentBuffer = buffers[nextBufferIndex];
			isAbleToStream = currentBuffer.FillFromStream(musicStream);
			if (!isAbleToStream)
				return;
			source.SubmitSourceBuffer(currentBuffer.XAudioBuffer, null);
			nextBufferIndex = (nextBufferIndex + 1) % NumberOfBuffers;
		}

		private int nextBufferIndex;
		private bool isAbleToStream;

		protected override void DisposeData()
		{
			if (musicStream == null)
				return;

			base.DisposeData();
			musicStream = null;

			if (source != null)
				DisposeSource();
		}

		private void DisposeSource()
		{
			for (int i = 0; i < NumberOfBuffers; i++)
				buffers[i].Dispose();

			source.FlushSourceBuffers();
			source.DestroyVoice();
			source.Dispose();
			source = null;
		}

		public override float DurationInSeconds
		{
			get { return musicStream.LengthInSeconds; }
		}

		public override float PositionInSeconds
		{
			get
			{
				float seconds = (float)DateTime.Now.Subtract(playStartTime).TotalSeconds;
				return MathExtensions.Round(seconds.Clamp(0f, DurationInSeconds), 2);
			}
		}
	}
}