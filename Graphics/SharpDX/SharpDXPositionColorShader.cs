using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace DeltaEngine.Graphics.SharpDX
{
	/// <summary>
	/// Shader for texturing, vector+color, stride: 16 bytes. Will be replaced with content shaders
	/// </summary>
	public sealed class SharpDXPositionColorShader : SharpDXShader
	{
		public SharpDXPositionColorShader(SharpDXDevice device)
			: base(device)
		{
			CreateVertexShader(VertexShaderByteCode);
			CreatePixelShader(PixelShaderByteCode);
			CreateInputLayout();
		}

		private static readonly byte[] VertexShaderByteCode = new byte[]
		{
			68, 88, 66, 67, 98, 208, 255, 51, 102, 210, 191, 56, 136, 188, 54, 83, 145, 94, 91, 22, 1, 0,
			0, 0, 32, 3, 0, 0, 5, 0, 0, 0, 52, 0, 0, 0, 16, 1, 0, 0, 100, 1, 0, 0, 184, 1, 0, 0, 164, 2,
			0, 0, 82, 68, 69, 70, 212, 0, 0, 0, 1, 0, 0, 0, 76, 0, 0, 0, 1, 0, 0, 0, 28, 0, 0, 0, 0, 4,
			254, 255, 0, 1, 0, 0, 160, 0, 0, 0, 60, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 67, 111, 110, 115, 116, 97, 110, 116, 66, 117, 102,
			102, 101, 114, 0, 171, 60, 0, 0, 0, 1, 0, 0, 0, 100, 0, 0, 0, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 124, 0, 0, 0, 0, 0, 0, 0, 64, 0, 0, 0, 2, 0, 0, 0, 144, 0, 0, 0, 0, 0, 0, 0, 87, 111,
			114, 108, 100, 86, 105, 101, 119, 80, 114, 111, 106, 101, 99, 116, 105, 111, 110, 0, 3, 0, 3,
			0, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 77, 105, 99, 114, 111, 115, 111, 102, 116, 32, 40, 82,
			41, 32, 72, 76, 83, 76, 32, 83, 104, 97, 100, 101, 114, 32, 67, 111, 109, 112, 105, 108, 101,
			114, 32, 57, 46, 50, 57, 46, 57, 53, 50, 46, 51, 49, 49, 49, 0, 171, 171, 171, 73, 83, 71, 78,
			76, 0, 0, 0, 2, 0, 0, 0, 8, 0, 0, 0, 56, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0,
			0, 15, 15, 0, 0, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 15, 15, 0, 0,
			83, 86, 95, 80, 79, 83, 73, 84, 73, 79, 78, 0, 67, 79, 76, 79, 82, 0, 171, 171, 79, 83, 71,
			78, 76, 0, 0, 0, 2, 0, 0, 0, 8, 0, 0, 0, 56, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 0, 0, 0, 0,
			0, 0, 0, 15, 0, 0, 0, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 15, 0, 0,
			0, 83, 86, 95, 80, 79, 83, 73, 84, 73, 79, 78, 0, 67, 79, 76, 79, 82, 0, 171, 171, 83, 72, 68,
			82, 228, 0, 0, 0, 64, 0, 1, 0, 57, 0, 0, 0, 89, 0, 0, 4, 70, 142, 32, 0, 0, 0, 0, 0, 4, 0, 0,
			0, 95, 0, 0, 3, 242, 16, 16, 0, 0, 0, 0, 0, 95, 0, 0, 3, 242, 16, 16, 0, 1, 0, 0, 0, 103, 0,
			0, 4, 242, 32, 16, 0, 0, 0, 0, 0, 1, 0, 0, 0, 101, 0, 0, 3, 242, 32, 16, 0, 1, 0, 0, 0, 17, 0,
			0, 8, 18, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142, 32, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 17, 0, 0, 8, 34, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142, 32, 0, 0, 0,
			0, 0, 1, 0, 0, 0, 17, 0, 0, 8, 66, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142,
			32, 0, 0, 0, 0, 0, 2, 0, 0, 0, 17, 0, 0, 8, 130, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0,
			0, 0, 70, 142, 32, 0, 0, 0, 0, 0, 3, 0, 0, 0, 54, 0, 0, 5, 242, 32, 16, 0, 1, 0, 0, 0, 70, 30,
			16, 0, 1, 0, 0, 0, 62, 0, 0, 1, 83, 84, 65, 84, 116, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 4, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		};

		private static readonly byte[] PixelShaderByteCode = new byte[]
		{
			68, 88, 66, 67, 150, 24, 98, 193, 223, 185, 25, 11, 105, 3, 199, 138, 10, 93, 81, 50, 1, 0, 0,
			0, 208, 1, 0, 0, 5, 0, 0, 0, 52, 0, 0, 0, 140, 0, 0, 0, 224, 0, 0, 0, 20, 1, 0, 0, 84, 1, 0,
			0, 82, 68, 69, 70, 80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 28, 0, 0, 0, 0, 4, 255,
			255, 0, 1, 0, 0, 28, 0, 0, 0, 77, 105, 99, 114, 111, 115, 111, 102, 116, 32, 40, 82, 41, 32,
			72, 76, 83, 76, 32, 83, 104, 97, 100, 101, 114, 32, 67, 111, 109, 112, 105, 108, 101, 114, 32,
			57, 46, 50, 57, 46, 57, 53, 50, 46, 51, 49, 49, 49, 0, 171, 171, 171, 73, 83, 71, 78, 76, 0,
			0, 0, 2, 0, 0, 0, 8, 0, 0, 0, 56, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 15,
			0, 0, 0, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 15, 15, 0, 0, 83, 86,
			95, 80, 79, 83, 73, 84, 73, 79, 78, 0, 67, 79, 76, 79, 82, 0, 171, 171, 79, 83, 71, 78, 44, 0,
			0, 0, 1, 0, 0, 0, 8, 0, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 15,
			0, 0, 0, 83, 86, 95, 84, 97, 114, 103, 101, 116, 0, 171, 171, 83, 72, 68, 82, 56, 0, 0, 0, 64,
			0, 0, 0, 14, 0, 0, 0, 98, 16, 0, 3, 242, 16, 16, 0, 1, 0, 0, 0, 101, 0, 0, 3, 242, 32, 16, 0,
			0, 0, 0, 0, 54, 0, 0, 5, 242, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 1, 0, 0, 0, 62, 0, 0, 1,
			83, 84, 65, 84, 116, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0
		};
		
		protected override void CreateInputLayout()
		{
			inputLayout = new InputLayout(device.NativeDevice, VertexShaderByteCode,
				new[]
				{
					new InputElement("SV_POSITION", 0, Format.R32G32B32_Float, 0, 0),
					new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0)
				});
		}
	}
}