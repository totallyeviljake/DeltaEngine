using DeltaEngine.Content.Tests;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	/// <summary>
	/// Confirms XmlContent can load from file
	/// </summary>
	public class XmlContentTests
	{
		[Test]
		public void LoadXmlContentFromMock()
		{
			new MockContentLoader(new ContentDataResolver());
			var xmlContent = ContentLoader.Load<XmlContent>("Test");
			Assert.False(xmlContent.IsDisposed);
			Assert.AreEqual("Root", xmlContent.Data.Name);
			Assert.AreEqual(1, xmlContent.Data.Children.Count);
			Assert.AreEqual("Hi", xmlContent.Data.Children[0].Name);
			xmlContent.Dispose();
		}

		//ncrunch: no coverage start
		[Test, Category("Slow"), Ignore]
		public void LoadXmlContentFromFile()
		{
			var xmlContent = ContentLoader.Load<XmlContent>("Test");
			Assert.False(xmlContent.IsDisposed);
			Assert.AreEqual("Root", xmlContent.Data.Name);
			Assert.AreEqual(1, xmlContent.Data.Children.Count);
			Assert.AreEqual("Hi", xmlContent.Data.Children[0].Name);
		}
	}
}