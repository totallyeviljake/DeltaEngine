using System.IO;

namespace DeltaEngine.Content.Json
{
	public class JsonContent : ContentData
	{
		public JsonContent(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData)
		{
			using (var stream = new StreamReader(fileData))
			{
				var text = stream.ReadToEnd();
				Data = new JsonNode(text);
			}
		}

		public JsonNode Data { get; private set; }

		protected override void DisposeData() {}
	}
}