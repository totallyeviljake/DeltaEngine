﻿using System;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.DXGI;
using SlimDX.Direct3D11;
using SlimD3D11 = SlimDX.Direct3D11;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXDrawShader : IDisposable
	{
		public SlimDXDrawShader(SlimDXDevice device)
		{
			this.device = device.Device;
			context = this.device.ImmediateContext;
			CreateVertexShader();
			CreateInputLayout();
			CreatePixelShader();
		}

		private readonly SlimD3D11.Device device;
		private readonly DeviceContext context;
		private ShaderSignature inputSignature;
		private InputLayout inputLayout;
		private VertexShader vertexShader;
		private PixelShader pixelShader;

		private static readonly byte[] VertexShaderByteCode = new byte[]
		{
			68, 88, 66, 67, 209, 215, 64, 19, 84, 170, 229, 194, 36, 27, 12, 113, 177, 92, 151, 8, 1, 0
			, 0, 0, 16, 3, 0, 0, 4, 0, 0, 0, 48, 0, 0, 0, 16, 1, 0, 0, 40, 2, 0, 0, 156, 2, 0, 0, 65,
			111, 110, 57, 216, 0, 0, 0, 216, 0, 0, 0, 0, 2, 254, 255, 164, 0, 0, 0, 52, 0, 0, 0, 1, 0,
			36, 0, 0, 0, 48, 0, 0, 0, 48, 0, 0, 0, 36, 0, 1, 0, 48, 0, 0, 0, 0, 0, 4, 0, 1, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 2, 254, 255, 31, 0, 0, 2, 5, 0, 0, 128, 0, 0, 15, 144, 31, 0, 0, 2, 5, 0,
			1, 128, 1, 0, 15, 144, 31, 0, 0, 2, 5, 0, 2, 128, 2, 0, 15, 144, 9, 0, 0, 3, 0, 0, 4, 192, 0
			, 0, 228, 144, 3, 0, 228, 160, 9, 0, 0, 3, 0, 0, 1, 128, 0, 0, 228, 144, 1, 0, 228, 160, 9,
			0, 0, 3, 0, 0, 2, 128, 0, 0, 228, 144, 2, 0, 228, 160, 9, 0, 0, 3, 0, 0, 4, 128, 0, 0, 228,
			144, 4, 0, 228, 160, 4, 0, 0, 4, 0, 0, 3, 192, 0, 0, 170, 128, 0, 0, 228, 160, 0, 0, 228,
			128, 1, 0, 0, 2, 0, 0, 8, 192, 0, 0, 170, 128, 1, 0, 0, 2, 0, 0, 3, 224, 1, 0, 228, 144, 1,
			0, 0, 2, 1, 0, 15, 224, 2, 0, 228, 144, 255, 255, 0, 0, 83, 72, 68, 82, 16, 1, 0, 0, 64, 0,
			1, 0, 68, 0, 0, 0, 89, 0, 0, 4, 70, 142, 32, 0, 0, 0, 0, 0, 4, 0, 0, 0, 95, 0, 0, 3, 242, 16
			, 16, 0, 0, 0, 0, 0, 95, 0, 0, 3, 50, 16, 16, 0, 1, 0, 0, 0, 95, 0, 0, 3, 242, 16, 16, 0, 2,
			0, 0, 0, 103, 0, 0, 4, 242, 32, 16, 0, 0, 0, 0, 0, 1, 0, 0, 0, 101, 0, 0, 3, 50, 32, 16, 0,
			1, 0, 0, 0, 101, 0, 0, 3, 242, 32, 16, 0, 2, 0, 0, 0, 17, 0, 0, 8, 18, 32, 16, 0, 0, 0, 0, 0
			, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17, 0, 0, 8, 34, 32, 16
			, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142, 32, 0, 0, 0, 0, 0, 1, 0, 0, 0, 17, 0, 0
			, 8, 66, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142, 32, 0, 0, 0, 0, 0, 2, 0,
			0, 0, 17, 0, 0, 8, 130, 32, 16, 0, 0, 0, 0, 0, 70, 30, 16, 0, 0, 0, 0, 0, 70, 142, 32, 0, 0,
			0, 0, 0, 3, 0, 0, 0, 54, 0, 0, 5, 50, 32, 16, 0, 1, 0, 0, 0, 70, 16, 16, 0, 1, 0, 0, 0, 54,
			0, 0, 5, 242, 32, 16, 0, 2, 0, 0, 0, 70, 30, 16, 0, 2, 0, 0, 0, 62, 0, 0, 1, 73, 83, 71, 78,
			108, 0, 0, 0, 3, 0, 0, 0, 8, 0, 0, 0, 80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0,
			0, 0, 15, 15, 0, 0, 92, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 3, 3, 0, 0,
			101, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 2, 0, 0, 0, 15, 15, 0, 0, 83, 86, 95, 80,
			111, 115, 105, 116, 105, 111, 110, 0, 84, 69, 88, 67, 79, 79, 82, 68, 0, 67, 79, 76, 79, 82,
			0, 171, 79, 83, 71, 78, 108, 0, 0, 0, 3, 0, 0, 0, 8, 0, 0, 0, 80, 0, 0, 0, 0, 0, 0, 0, 1, 0,
			0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 92, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0,
			1, 0, 0, 0, 3, 12, 0, 0, 101, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 2, 0, 0, 0, 15, 0
			, 0, 0, 83, 86, 95, 80, 111, 115, 105, 116, 105, 111, 110, 0, 84, 69, 88, 67, 79, 79, 82, 68
			, 0, 67, 79, 76, 79, 82, 0, 171
		};

		private static readonly byte[] PixelShaderByteCode = new byte[]
		{
			68, 88, 66, 67, 160, 139, 42, 206, 70, 216, 150, 93, 211, 169, 240, 24, 143, 244, 31, 121,
			1, 0, 0, 0, 12, 2, 0, 0, 4, 0, 0, 0, 48, 0, 0, 0, 184, 0, 0, 0, 100, 1, 0, 0, 216, 1, 0, 0,
			65, 111, 110, 57, 128, 0, 0, 0, 128, 0, 0, 0, 0, 2, 255, 255, 88, 0, 0, 0, 40, 0, 0, 0, 0, 0
			, 40, 0, 0, 0, 40, 0, 0, 0, 40, 0, 1, 0, 36, 0, 0, 0, 40, 0, 0, 0, 0, 0, 0, 2, 255, 255, 31,
			0, 0, 2, 0, 0, 0, 128, 0, 0, 3, 176, 31, 0, 0, 2, 0, 0, 0, 128, 1, 0, 15, 176, 31, 0, 0, 2,
			0, 0, 0, 144, 0, 8, 15, 160, 66, 0, 0, 3, 0, 0, 15, 128, 0, 0, 228, 176, 0, 8, 228, 160, 5,
			0, 0, 3, 0, 0, 15, 128, 0, 0, 228, 128, 1, 0, 228, 176, 1, 0, 0, 2, 0, 8, 15, 128, 0, 0, 228
			, 128, 255, 255, 0, 0, 83, 72, 68, 82, 164, 0, 0, 0, 64, 0, 0, 0, 41, 0, 0, 0, 89, 0, 0, 4,
			70, 142, 32, 0, 0, 0, 0, 0, 1, 0, 0, 0, 90, 0, 0, 3, 0, 96, 16, 0, 0, 0, 0, 0, 88, 24, 0, 4,
			0, 112, 16, 0, 0, 0, 0, 0, 85, 85, 0, 0, 98, 16, 0, 3, 50, 16, 16, 0, 1, 0, 0, 0, 98, 16, 0,
			3, 242, 16, 16, 0, 2, 0, 0, 0, 101, 0, 0, 3, 242, 32, 16, 0, 0, 0, 0, 0, 104, 0, 0, 2, 1, 0,
			0, 0, 69, 0, 0, 9, 242, 0, 16, 0, 0, 0, 0, 0, 70, 16, 16, 0, 1, 0, 0, 0, 70, 126, 16, 0, 0,
			0, 0, 0, 0, 96, 16, 0, 0, 0, 0, 0, 56, 0, 0, 7, 242, 32, 16, 0, 0, 0, 0, 0, 70, 14, 16, 0, 0
			, 0, 0, 0, 70, 30, 16, 0, 2, 0, 0, 0, 62, 0, 0, 1, 73, 83, 71, 78, 108, 0, 0, 0, 3, 0, 0, 0,
			8, 0, 0, 0, 80, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 92, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 3, 3, 0, 0, 101, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 3, 0, 0, 0, 2, 0, 0, 0, 15, 15, 0, 0, 83, 86, 95, 80, 111, 115, 105, 116, 105,
			111, 110, 0, 84, 69, 88, 67, 79, 79, 82, 68, 0, 67, 79, 76, 79, 82, 0, 171, 79, 83, 71, 78,
			44, 0, 0, 0, 1, 0, 0, 0, 8, 0, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0,
			0, 0, 15, 0, 0, 0, 83, 86, 95, 84, 97, 114, 103, 101, 116, 0, 171, 171
		};

		private void CreateVertexShader()
		{
			var bytecode = GetShaderBytecodeObject(VertexShaderByteCode);
			inputSignature = ShaderSignature.GetInputSignature(bytecode);
			vertexShader = new VertexShader(device, bytecode);
		}

		private void CreatePixelShader()
		{
			var bytecode = GetShaderBytecodeObject(PixelShaderByteCode);
			pixelShader = new PixelShader(device, bytecode);
		}

		private static ShaderBytecode GetShaderBytecodeObject(byte[] shaderBytecode)
		{
			DataStream dataStream = new DataStream(shaderBytecode.Length, true, true);
			dataStream.Write(shaderBytecode, 0, shaderBytecode.Length);
			return new ShaderBytecode(dataStream);
		}

		private void CreateInputLayout()
		{
			var elements = new[]
			{
					new InputElement("SV_POSITION", 0, Format.R32G32B32_Float, 0, 0),
					new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0),
					new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)				
			};

			inputLayout = new InputLayout(device, inputSignature, elements);
			context.InputAssembler.InputLayout = inputLayout;
		}

		public void Apply()
		{
			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);
			context.InputAssembler.InputLayout = inputLayout;
		}

		public void Dispose()
		{
			pixelShader.Dispose();
			inputLayout.Dispose();
			inputSignature.Dispose();
			vertexShader.Dispose();
		}
	}
}
