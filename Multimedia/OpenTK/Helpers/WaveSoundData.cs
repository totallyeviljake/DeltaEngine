using System;
using System.IO;

namespace DeltaEngine.Multimedia.OpenTK.Helpers
{
	/// <summary>
	/// Helper class to parse the headers and chunks of a wave file and
	/// calling the needed converter classes.
	/// </summary>
	public class WaveSoundData
	{
		public WaveSoundData(string filepath)
		{
			using (var reader = new BinaryReader(File.OpenRead(filepath)))
				ParseWaveData(reader);
			SetOpenAlFormat();
			if (waveFormat != WaveFormat.Pcm)
				ConvertToPcm();
		}

		public WaveSoundData(BinaryReader reader)
		{
			ParseWaveData(reader);
			SetOpenAlFormat();
			if (waveFormat != WaveFormat.Pcm)
				ConvertToPcm();
		}

		private WaveFormat waveFormat;

		private void ParseWaveData(BinaryReader reader)
		{
			ThrowIfInvalidHeader(reader);
			while (IsEndOfStream(reader) == false)
				ParseNextChunk(reader);
		}

		private static bool IsEndOfStream(BinaryReader reader)
		{
			return reader.BaseStream.Position >= reader.BaseStream.Length;
		}

		private void ParseNextChunk(BinaryReader reader)
		{
			string identifier = ReadFourCharIdentifier(reader).ToLower();
			if (identifier == "\0" || reader.BaseStream.Position == reader.BaseStream.Length)
				return;

			int chunkLength = reader.ReadInt32();
			long positionBeforeReading = reader.BaseStream.Position;
			if (identifier == "fmt ")
				ReadFmtChunk(reader, chunkLength);
			if (identifier == "data")
				BufferData = reader.ReadBytes(chunkLength);

			SkipUnreadChunkData(reader, positionBeforeReading, chunkLength);
		}

		private void ReadFmtChunk(BinaryReader reader, int chunkLength)
		{
			waveFormat = (WaveFormat)reader.ReadInt16();
			Channels = reader.ReadInt16();
			SampleRate = reader.ReadInt32();
			reader.ReadInt32();
			blockAlign = reader.ReadInt16();
			bitsPerSample = reader.ReadInt16();
			if (chunkLength > 18)
				ReadExtendedFormatFmtData(reader);
		}

		private int bitsPerSample;
		private short blockAlign;

		private void ReadExtendedFormatFmtData(BinaryReader reader)
		{
			extensionSamplesPerBlock = reader.ReadInt16();
			reader.ReadInt32();
			var extFormat = (WaveFormat)reader.ReadInt16();
			waveFormat = waveFormat < 0 ? extFormat : waveFormat;
			reader.ReadBytes(14);
		}

		private short extensionSamplesPerBlock;

		private static void SkipUnreadChunkData(BinaryReader reader, long positionBeforeReading,
			int chunkLength)
		{
			long lengthRead = reader.BaseStream.Position - positionBeforeReading;
			if (lengthRead != chunkLength)
				reader.BaseStream.Seek(chunkLength - lengthRead, SeekOrigin.Current);
		}

		private void SetOpenAlFormat()
		{
			if (Channels == 1)
				Format = bitsPerSample == 8 ? AudioFormat.Mono8 : AudioFormat.Mono16;
			else
				Format = bitsPerSample == 8 ? AudioFormat.Stereo8 : AudioFormat.Stereo16;
		}

		private void ConvertToPcm()
		{
			if (waveFormat == WaveFormat.MsAdpcm)
				ConvertMsAdpcmToPcm();
			else if (waveFormat == WaveFormat.IeeeFloat)
				ConvertIeeeFloatToPcm();
			else
				throw new NotSupportedException("The Wave format " + waveFormat +
					" is not supported yet. Unable to load!");
		}

		private void ConvertMsAdpcmToPcm()
		{
			var converter = new MsAdpcmConverter(Channels, extensionSamplesPerBlock, blockAlign);
			BufferData = converter.ConvertToPcm(BufferData);
		}

		private void ConvertIeeeFloatToPcm()
		{
			var converter = new IeeeFloatConverter(bitsPerSample);
			BufferData = converter.ConvertToPcm(BufferData);
		}

		private static void ThrowIfInvalidHeader(BinaryReader reader)
		{
			if (ReadFourCharIdentifier(reader) != "RIFF")
				throw new NotSupportedException("The file is no RIFF(Wave) file.");

			reader.ReadInt32();
			if (ReadFourCharIdentifier(reader) != "WAVE")
				throw new NotSupportedException("The file is no RIFF(Wave) file.");
		}

		private static string ReadFourCharIdentifier(BinaryReader reader)
		{
			return new string(reader.ReadChars(4));
		}

		public int Channels { get; private set; }
		public int SampleRate { get; private set; }
		public AudioFormat Format { get; private set; }
		public byte[] BufferData { get; private set; }
	}
}