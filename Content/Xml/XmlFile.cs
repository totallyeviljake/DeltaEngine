using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Loads and saves XmlData to file
	/// </summary>
	public class XmlFile
	{
		public XmlFile(XmlData xmlData)
		{
			Root = xmlData;
		}

		public XmlData Root { get; private set; }

		public XmlFile(string filePath)
		{
			using (var s = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				Root = new XmlData(XDocument.Load(s).Root);
		}

		public void Save(string filePath)
		{
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
				FileShare.ReadWrite))
			using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
				Root.XRootElement.Document.Save(writer);
		}

		public Stream ToMemoryStream()
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream, new UTF8Encoding(false));
			Root.XRootElement.Document.Save(writer);
			stream.Position = 0;
			return stream;
		}
	}
}