using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleCreatorTests
	{
		[SetUp]
		public void Init()
		{
			sampleCreator = new SampleCreator(CreateMockSamplesFileSystem());
			sampleCreator.SamplesPath = "Samples";
			sampleCreator.DeltaEngineRootPath = "";
			sampleCreator.FallbackPath = "";
		}

		private SampleCreator sampleCreator;

		private static IFileSystem CreateMockSamplesFileSystem()
		{
			var files = new Dictionary<string, MockFileData>();
			files.Add(@"Samples\EmptyGame\EmptyGame.csproj", new MockFileData(""));
			files.Add(@"Samples\EmptyGame\bin\Debug\EmptyGame.exe", new MockFileData(""));
			files.Add(@"Samples\EmptyGame\Tests\EmptyGame.Tests.csproj", new MockFileData(""));
			files.Add(@"C:\Code\DeltaEngine\Editor\SampleBrowser\Tests\Assemblies\EmptyGame.Tests.dll",
				new MockFileData(GetTestAssemblyData()));
			return new MockFileSystem(files);
		}

		private static string GetTestAssemblyData()
		{
			string assembly = Path.GetFullPath("TestAssembly.dll");
			return File.ReadAllText(assembly);
		}

		[Test, Category("Slow")]
		public void CreateSampleFromMockAssembly()
		{
			Assert.AreEqual(0, sampleCreator.Samples.Count);
			sampleCreator.CreateSamples();
			Assert.AreEqual(1, sampleCreator.Samples.Count);
			Assert.AreEqual("EmptyGame", sampleCreator.Samples[0].Title);
			Assert.AreEqual("Sample Game", sampleCreator.Samples[0].Description);
			Assert.AreEqual("Game", sampleCreator.Samples[0].Category.ToString());
			Assert.AreEqual("http://DeltaEngine.net/Content/Icons/EmptyGame.png",
				sampleCreator.Samples[0].ImageFilePath);
			Assert.AreEqual(@"C:\Foo\Bar\Samples\EmptyGame\EmptyGame.csproj",
				sampleCreator.Samples[0].ProjectFilePath);
			Assert.AreEqual(@"C:\Foo\Bar\Samples\EmptyGame\bin\Debug\EmptyGame.exe",
				sampleCreator.Samples[0].AssemblyFilePath);
			Assert.AreEqual("", sampleCreator.Samples[0].EntryClass);
			Assert.AreEqual("", sampleCreator.Samples[0].EntryMethod);
		}
	}
}