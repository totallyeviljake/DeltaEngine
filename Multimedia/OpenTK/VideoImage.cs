using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia.AviVideo;
using DeltaEngine.Platforms;
using OpenTK.Graphics.OpenGL;

namespace DeltaEngine.Multimedia.OpenTK
{
	[IgnoreForResolver]
	public class VideoImage : Image
	{
		public VideoImage(Device device)
			: base("<VideoTexture>")
		{
			GL.GenTextures(1, out glHandle);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
		}

		protected const int InvalidHandle = -1;
		protected int glHandle = InvalidHandle;

		protected override void LoadImage(Stream fileData) {}
		protected override void CreateDefaultTexture() {}

		protected override void DisposeData()
		{
			if (glHandle == InvalidHandle)
				return;

			GL.DeleteTexture(glHandle);
			glHandle = InvalidHandle;
		}

		public void Draw(VertexPositionColorTextured[] vertices)
		{
			vertices[0] = new VertexPositionColorTextured(vertices[0].Position, vertices[0].Color,
				Point.UnitY);
			vertices[1] = new VertexPositionColorTextured(vertices[1].Position, vertices[1].Color,
				Point.One);
			vertices[2] = new VertexPositionColorTextured(vertices[2].Position, vertices[2].Color,
				Point.UnitX);
			vertices[3] = new VertexPositionColorTextured(vertices[3].Position, vertices[3].Color,
				Point.Zero);

			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
		}

		public void UpdateTexture(VideoStream stream, int frameIndex)
		{
			//size = new Size(stream.Width, stream.Height);
			GL.BindTexture(TextureTarget.Texture2D, glHandle);
			AviInterop.BitmapInfoHeader bitmapHeader;
			byte[] pixelData = stream.GetStreamData(frameIndex, out bitmapHeader);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, bitmapHeader.biWidth,
				bitmapHeader.biHeight, 0, PixelFormat.Bgr, PixelType.UnsignedByte, pixelData);
			SetSamplerState();
		}

		protected override void SetSamplerState()
		{
			SetTexture2DParameter(TextureParameterName.TextureMinFilter,
				DisableLinearFiltering ? All.Nearest : All.Linear);
			SetTexture2DParameter(TextureParameterName.TextureMagFilter,
				DisableLinearFiltering ? All.Nearest : All.Linear);
			SetTexture2DParameter(TextureParameterName.TextureWrapS, All.ClampToEdge);
			SetTexture2DParameter(TextureParameterName.TextureWrapT, All.ClampToEdge);
		}

		private static void SetTexture2DParameter(TextureParameterName name, All value)
		{
			GL.TexParameter(TextureTarget.Texture2D, name, (int)value);
		}
	}
}