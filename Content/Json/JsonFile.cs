using System.IO;

namespace DeltaEngine.Content.Json
{
	/// <summary>
	/// Allows to load json files from disk by utilizing the JsonNode parser.
	/// </summary>
	public class JsonFile
	{
		public JsonFile(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFound(filePath);

			Root = new JsonNode(File.ReadAllText(filePath));
		}

		public class FileNotFound : FileNotFoundException
		{
			public FileNotFound(string filePath)
				: base("", filePath) {}
		}

		public JsonNode Root { get; private set; }
	}
}