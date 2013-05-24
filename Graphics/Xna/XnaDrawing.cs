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
			InitializeVertexBuffers();
			InitializeBasicEffect();
		}

		private new readonly XnaDevice device;
		private readonly Window window;

		private void InitializeVertexBuffers()
		{
			positionColorVertexBuffer = new XnaCircularBuffer(VertexBufferSize,
				typeof(XnaGraphics.VertexPositionColor), device);
			positionColorUvVertexBuffer = new XnaCircularBuffer(VertexBufferSize,
				typeof(VertexPositionColorTexture), device);
		}

		private XnaCircularBuffer positionColorVertexBuffer;
		private XnaCircularBuffer positionColorUvVertexBuffer;
		private const int VertexBufferSize = 1024;

		private void InitializeBasicEffect()
		{
			basicEffect = new BasicEffect(device.NativeDevice);
			UpdateProjectionMatrix(window.ViewportPixelSize);
			window.ViewportSizeChanged += UpdateProjectionMatrix;
			basicEffect.View = Matrix.CreateTranslation(-0.5f, -0.5f, 0.0f);
			device.NativeDevice.BlendState = BlendState.NonPremultiplied;
		}

		public override void Dispose()
		{
			if (basicEffect != null)
				basicEffect.Dispose();
		}

		private BasicEffect basicEffect;

		public override void EnableTexturing(Image image)
		{
			device.NativeDevice.Textures[0] = (image as XnaImage).NativeTexture;
			device.NativeDevice.SamplerStates[0] = image.DisableLinearFiltering
				? SamplerState.PointClamp : SamplerState.LinearClamp;
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = device.NativeDevice.Textures[0] as Texture2D;
		}

		public override void DisableTexturing()
		{
			basicEffect.TextureEnabled = false;
		}

		public override void SetBlending(BlendMode blendMode)
		{
			device.NativeDevice.BlendState = GetXnaBlendState(blendMode);
		}

		private static BlendState GetXnaBlendState(BlendMode blendMode)
		{
			if (blendMode == BlendMode.Additive)
				return BlendState.Additive;

			if (blendMode == BlendMode.Opaque)
				return BlendState.Opaque;

			return BlendState.AlphaBlend;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			if (!positionColorUvVertexBuffer.IsCreated)
				positionColorUvVertexBuffer.Create();

			positionColorUvVertexBuffer.SetVertexData(vertices);
			device.NativeDevice.SetVertexBuffer(positionColorUvVertexBuffer.NativeBuffer);
			Draw(positionColorUvVertexBuffer, mode, vertices.Length);
		}

		private void ApplyEffect()
		{
			basicEffect.VertexColorEnabled = true;
			basicEffect.TextureEnabled = device.NativeDevice.Textures[0] != null;
			if (basicEffect.TextureEnabled)
				basicEffect.Texture = device.NativeDevice.Textures[0] as Texture2D;

			basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		private void Draw(XnaCircularBuffer buffer, VerticesMode mode, int length)
		{
			ApplyEffect();
			var primitiveMode = Convert(mode);
			var primitiveCount =
				GetPrimitiveCount(lastIndicesCount == -1 ? length : lastIndicesCount, primitiveMode);
			if (lastIndicesCount == -1)
				DrawPrimitives(buffer, primitiveMode, primitiveCount);
			else
				DrawIndexedPrimitives(buffer, primitiveMode, length, primitiveCount);
		}

		private int lastIndicesCount = -1;

		private void DrawPrimitives(XnaCircularBuffer buffer, PrimitiveType primitiveType,
			int primitiveCount)
		{
			device.NativeDevice.DrawPrimitives(primitiveType, buffer.Offset / buffer.VertexSize,
				primitiveCount);
		}

		private void DrawIndexedPrimitives(XnaCircularBuffer buffer, PrimitiveType primitiveType,
			int length, int primitiveCount)
		{
			device.NativeDevice.DrawIndexedPrimitives(primitiveType, buffer.Offset / buffer.VertexSize,
				0, length, 0, primitiveCount);
		}

		private static PrimitiveType Convert(VerticesMode mode)
		{
			return mode == VerticesMode.Triangles ? PrimitiveType.TriangleList : PrimitiveType.LineList;
		}

		private static int GetPrimitiveCount(int numVerticesOrIndices, PrimitiveType primitiveType)
		{
			return primitiveType == PrimitiveType.LineList
				? numVerticesOrIndices / 2 : numVerticesOrIndices / 3;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			if (!positionColorVertexBuffer.IsCreated)
				positionColorVertexBuffer.Create();

			positionColorVertexBuffer.SetVertexData(vertices);
			device.NativeDevice.SetVertexBuffer(positionColorVertexBuffer.NativeBuffer);
			Draw(positionColorVertexBuffer, mode, vertices.Length);
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			if (indexBuffer == null)
				CreateIndexBuffer();

			device.NativeDevice.Indices = null;
			indexBuffer.SetData(indices);
			device.NativeDevice.Indices = indexBuffer;
			lastIndicesCount = usedIndicesCount;
		}

		public override void DisableIndices()
		{
			lastIndicesCount = -1;
		}

		private DynamicIndexBuffer indexBuffer;
		
		private void UpdateProjectionMatrix(Size newViewportSize)
		{
			basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, newViewportSize.Width,
				newViewportSize.Height, 0, 0, 1);
		}

		private void CreateIndexBuffer(int indexCount = 600)
		{
			indexBuffer = new DynamicIndexBuffer(device.NativeDevice, IndexElementSize.SixteenBits,
				indexCount, BufferUsage.WriteOnly);
		}
	}
}