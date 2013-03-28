﻿using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework.Graphics;
using XnaGraphics = Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaDrawing : Drawing
	{
		public XnaDrawing(XnaDevice device, Window window)
			: base(device)
		{
			this.device = device;
			this.window = window;
			window.ViewportSizeChanged += Reset;
			InitializeBuffers();
		}

		private void Reset(Size obj)
		{
			lastTexture = null;
			lastIndices = null;
		}

		private new readonly XnaDevice device;
		private readonly Window window;

		private void InitializeBuffers()
		{
			positionColorVertexBuffer = new CircularBuffer(NumberOfVertexBufferChunks);
			positionColorUvVertexBuffer = new CircularBuffer(NumberOfVertexBufferChunks);
		}

		private const int NumberOfVertexBufferChunks = 4;
		private CircularBuffer positionColorVertexBuffer;
		private CircularBuffer positionColorUvVertexBuffer;

		public override void Dispose()
		{
			if (basicEffect != null)
				basicEffect.Dispose();
		}

		public override void DisableTexturing()
		{
			lastTexture = null;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreatePositionColorTextureBuffer();
			SetVerticesData(positionColorUvVertexBuffer, vertices);
			BindVertexBuffer(positionColorUvVertexBuffer.Handle);
			if (lastTexture != device.NativeDevice.Textures[0])
			{
				ApplyEffect(true);
				lastTexture = device.NativeDevice.Textures[0];
			}

			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length, positionColorUvVertexBuffer);
			else
				DoDrawIndexed(mode, vertices.Length, lastIndicesCount, positionColorUvVertexBuffer);

			positionColorUvVertexBuffer.SelectNextChunk();
		}

		private static void SetVerticesData(CircularBuffer buffer, 
			VertexPositionColorTextured[] vertices)
		{
			int vertexSize = Marshal.SizeOf(vertices[0]);
			int count = vertices.Length;
			buffer.Handle.SetData(vertexSize * count * buffer.CurrentChunk, vertices, 0, count,
				vertexSize, SetDataOptions.Discard);
		}

		private Texture lastTexture;

		private void CheckCreatePositionColorTextureBuffer(int vertexCount = 8192)
		{
			if (positionColorUvVertexBuffer.Handle != null)
				return;

			positionColorUvVertexBuffer.Handle = new DynamicVertexBuffer(device.NativeDevice,
				typeof(VertexPositionColorTexture), 
				vertexCount * positionColorUvVertexBuffer.NumberOfChunks, BufferUsage.WriteOnly);
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			CheckCreatePositionColorBuffer();
			device.NativeDevice.SetVertexBuffers(null);
			SetVerticesData(positionColorVertexBuffer, vertices);
			BindVertexBuffer(positionColorVertexBuffer.Handle);
			ApplyEffect(true);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length, positionColorVertexBuffer);
			else
				DoDrawIndexed(mode, vertices.Length, lastIndicesCount, positionColorVertexBuffer);

			positionColorVertexBuffer.SelectNextChunk();
		}

		private static void SetVerticesData(CircularBuffer buffer, VertexPositionColor[] vertices)
		{
			int vertexSize = Marshal.SizeOf(vertices[0]);
			int count = vertices.Length;
			buffer.Handle.SetData(vertexSize * count * buffer.CurrentChunk, vertices, 0, count,
				vertexSize, SetDataOptions.Discard);
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			CheckCreateIndexBuffer();
			if (lastIndices == indices)
				return;

			if (device.NativeDevice.Indices == indexBuffer)
				device.NativeDevice.Indices = null;

			indexBuffer.SetData(indices);
			device.NativeDevice.Indices = indexBuffer;
			lastIndices = indices;
			lastIndicesCount = usedIndicesCount;
		}

		private short[] lastIndices;
		private int lastIndicesCount = -1;

		private void BindVertexBuffer(VertexBuffer vertexBuffer)
		{
			device.NativeDevice.SetVertexBuffer(vertexBuffer);
		}

		private void ApplyEffect(bool vertexColorEnabled)
		{
			InitializeBasicEffectIfRequired();
			basicEffect.VertexColorEnabled = vertexColorEnabled;
			CheckEnableEffectTexture();
			basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		private void InitializeBasicEffectIfRequired()
		{
			if (basicEffect != null)
				return;
			
			basicEffect = new BasicEffect(device.NativeDevice);
			UpdateProjectionMatrix(window.ViewportPixelSize);
			window.ViewportSizeChanged += UpdateProjectionMatrix;
			FixHalfPixelOffset();
			device.NativeDevice.BlendState = BlendState.NonPremultiplied;
		}

		private BasicEffect basicEffect;

		private void UpdateProjectionMatrix(Size newViewportSize)
		{
			basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, newViewportSize.Width,
				newViewportSize.Height, 0, 0, 1);
		}

		private void FixHalfPixelOffset()
		{
			basicEffect.View = Matrix.CreateTranslation(-0.5f, -0.5f, 0.0f);
		}

		private void CheckEnableEffectTexture()
		{
			if (device.NativeDevice.Textures[0] == null)
				basicEffect.TextureEnabled = false;
			else
			{
				basicEffect.TextureEnabled = true;
				basicEffect.Texture = device.NativeDevice.Textures[0] as Texture2D;
			}
		}

		private void DoDraw(VerticesMode mode, int verticesCount, CircularBuffer vertexBuffer)
		{
			var primitiveMode = Convert(mode);
			var primitiveCount = GetPrimitiveCount(verticesCount, primitiveMode);
			var verticesPerPrimitive = mode == VerticesMode.Triangles ? 3 : 2;
			device.NativeDevice.DrawPrimitives(primitiveMode, 
				verticesPerPrimitive * primitiveCount * vertexBuffer.CurrentChunk, primitiveCount);
		}

		private void DoDrawIndexed(VerticesMode mode, int verticesCount, int indicesCount,
			CircularBuffer vertexBuffer)
		{
			var primitiveMode = Convert(mode);
			var primitiveCount = GetPrimitiveCount(indicesCount, primitiveMode);
			device.NativeDevice.DrawIndexedPrimitives(primitiveMode, 
				VerticesPerRectangle * vertexBuffer.CurrentChunk, 0, verticesCount, 0, primitiveCount);
		}

		private const int VerticesPerRectangle = 4;
		
		private void CheckCreatePositionColorBuffer(int vertexCount = 400)
		{
			if (positionColorVertexBuffer.Handle != null)
				return;

			positionColorVertexBuffer.Handle = new DynamicVertexBuffer(device.NativeDevice,
				typeof(XnaGraphics.VertexPositionColor), 
				vertexCount * positionColorVertexBuffer.NumberOfChunks, BufferUsage.WriteOnly);
		}

		private void CheckCreateIndexBuffer(int indexCount = 600)
		{
			if (indexBuffer != null)
				return;

			indexBuffer = new DynamicIndexBuffer(device.NativeDevice, IndexElementSize.SixteenBits,
				indexCount, BufferUsage.WriteOnly);
		}

		private DynamicIndexBuffer indexBuffer;

		private static PrimitiveType Convert(VerticesMode mode)
		{
			switch (mode)
			{
			case VerticesMode.Lines:
				return PrimitiveType.LineList;
			default:
				return PrimitiveType.TriangleList;
			}
		}

		private static int GetPrimitiveCount(int numVerticesOrIndices, PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
			case PrimitiveType.LineList:
				return numVerticesOrIndices / 2;
			default:
				return numVerticesOrIndices / 3;
			}
		}
	}
}