using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class BinaryDataLoadSaveTests
	{
		[Test]
		public void SaveAndLoadPrimitiveDataTypes()
		{
			SaveDataTypeAndLoadAgain((sbyte)-8);
			SaveDataTypeAndLoadAgain(-8);
			SaveDataTypeAndLoadAgain((Int16)8);
			SaveDataTypeAndLoadAgain((UInt16)8);
			SaveDataTypeAndLoadAgain((long)-8);
			SaveDataTypeAndLoadAgain((uint)8);
			SaveDataTypeAndLoadAgain((ulong)8);
			SaveDataTypeAndLoadAgain(3.4f);
			SaveDataTypeAndLoadAgain(8.4);
			SaveDataTypeAndLoadAgain(false);
		}

		private static void SaveDataTypeAndLoadAgain<Primitive>(Primitive input)
		{
			var data = SaveDataIntoMemoryStream(input);
			var output = LoadDataFromMemoryStream<Primitive>(data);
			Assert.AreEqual(input, output);
		}

		private static MemoryStream SaveDataIntoMemoryStream<DataType>(DataType input)
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			BinaryDataSaver.TrySaveData(input, typeof(DataType), writer);
			return data;
		}

		private static DataType LoadDataFromMemoryStream<DataType>(MemoryStream data)
		{
			data.Seek(0, SeekOrigin.Begin);
			var reader = new BinaryReader(data);
			return (DataType)BinaryDataLoader.TryCreateAndLoad(typeof(DataType), reader);
		}

		[Test]
		public void SaveAndLoadOtherDatatypes()
		{
			SaveDataTypeAndLoadAgain("Hi");
			SaveDataTypeAndLoadAgain('x');
			SaveDataTypeAndLoadAgain((decimal)8.4);
			SaveDataTypeAndLoadAgain("asdf".ToCharArray());
			SaveDataTypeAndLoadAgain("asdf".ToByteArray());
			SaveDataTypeAndLoadAgain(TestEnum.SomeFlag);
		}

		private enum TestEnum
		{
			SomeFlag,
		}

		[Test]
		public void SaveAndLoadLists()
		{
			SaveAndLoadList(new List<int> { 2, 4, 7, 15 });
			SaveAndLoadList(new List<Object> { 2, 0.5f, "Hello" });
		}

		private static void SaveAndLoadList<Primitive>(List<Primitive> listData)
		{
			var data = SaveDataIntoMemoryStream(listData);
			var retrievedList = LoadDataFromMemoryStream<List<Primitive>>(data);
			Assert.AreEqual(listData.Count, retrievedList.Count);
			if (typeof(Primitive).IsValueType)
				Assert.IsTrue(listData.Compare(retrievedList));

			for (int index = 0; index < listData.Count; index ++)
				Assert.AreEqual(listData[index].GetType(), retrievedList[index].GetType());
		}

		[Test]
		public void SaveAndLoadArrays()
		{
			SaveAndLoadArray(new[] { 2, 4, 7, 15 });
			SaveAndLoadArray(new object[] { 2, 0.5f, "Hello" });
			SaveAndLoadArray(new byte[] { 5, 6, 7 });
			SaveAndLoadArray(new byte[0]);
		}

		private static void SaveAndLoadArray<T>(T[] array)
		{
			var data = SaveDataIntoMemoryStream(array);
			var retrievedArray = LoadDataFromMemoryStream<T[]>(data);
			Assert.AreEqual(array.Length, retrievedArray.Length);
			Assert.IsTrue(array.Compare(retrievedArray));
		}

		[Test]
		public void SaveAndLoadArraysContainingNullValues()
		{
			SaveDataIntoMemoryStream(new object[] { null });
			SaveDataIntoMemoryStream(new object[] { 0, 'a', "hallo", null });
		}

		[Test]
		public void SaveAndLoadClassWithArrays()
		{
			var instance = new ClassWithArrays();
			var data = SaveDataIntoMemoryStream(instance);
			var retrieved = LoadDataFromMemoryStream<ClassWithArrays>(data);
			Assert.IsTrue(retrieved.byteData.Compare(new byte[] { 1, 2, 3, 4, 5 }),
				retrieved.byteData.ToText());
			Assert.IsTrue(retrieved.charData.Compare(new[] { 'a', 'b', 'c' }),
				retrieved.charData.ToText());
			Assert.IsTrue(retrieved.intData.Compare(new[] { 10, 20, 30 }), retrieved.intData.ToText());
			Assert.IsTrue(retrieved.stringData.Compare(new[] { "Hi", "there" }),
				retrieved.stringData.ToText());
			Assert.IsTrue(retrieved.enumData.Compare(new[] { DayOfWeek.Monday, DayOfWeek.Sunday }),
				retrieved.enumData.ToText());
			Assert.IsTrue(retrieved.byteEnumData.Compare(new[] { ByteEnum.Normal, ByteEnum.High }),
				retrieved.byteEnumData.ToText());
		}
	
		private class ClassWithArrays
		{
			public readonly byte[] byteData = { 1, 2, 3, 4, 5 };
			public readonly char[] charData = { 'a', 'b', 'c' };
			public readonly int[] intData = { 10, 20, 30 };
			public readonly string[] stringData = { "Hi", "there" };
			public readonly DayOfWeek[] enumData = { DayOfWeek.Monday, DayOfWeek.Sunday };
			public readonly ByteEnum[] byteEnumData = { ByteEnum.Normal, ByteEnum.High };
		}
	
		enum ByteEnum : byte
		{
			Normal,
			High,
		}
		
		[Test]
		public void SaveAndLoadClassWithEmptyByteArray()
		{
			var instance = new ClassWithByteArray { data = new byte[] { 1, 2, 3 } };
			var data = SaveDataIntoMemoryStream(instance);
			var retrieved = LoadDataFromMemoryStream<ClassWithByteArray>(data);
			Assert.IsTrue(instance.data.Compare(retrieved.data));
		}

		private class ClassWithByteArray
		{
			public byte[] data;
		}

		[Test]
		public void SaveAndLoadExplicitLayoutStruct()
		{
			var explicitLayoutTest = new ExplicitLayoutTestClass
			{
				someValue = 8,
				anotherValue = 5,
				unionValue = 7
			};
			var data = SaveDataIntoMemoryStream(explicitLayoutTest);
			var retrieved = LoadDataFromMemoryStream<ExplicitLayoutTestClass>(data);
			Assert.AreEqual(8, retrieved.someValue);
			Assert.AreEqual(7, retrieved.anotherValue);
			Assert.AreEqual(7, retrieved.unionValue);
		}

		[StructLayout(LayoutKind.Explicit)]
		private class ExplicitLayoutTestClass
		{
			[FieldOffset(0)]
			public int someValue;
			[FieldOffset(4)]
			public int anotherValue;
			[FieldOffset(4)]
			public int unionValue;
		}

		[Test]
		public void SaveAndLoadClassWithAnotherClassInside()
		{
			var instance = new ClassWithAnotherClassInside
			{
				Number = 17,
				Data = new ClassWithAnotherClassInside.InnerDerivedClass
				{
					Value = 1.5,
					additionalFlag = true
				}
			};
			var data = SaveDataIntoMemoryStream(instance);
			var retrieved = LoadDataFromMemoryStream<ClassWithAnotherClassInside>(data);
			Assert.AreEqual(instance.Number, retrieved.Number);
			Assert.AreEqual(instance.Data.Value, retrieved.Data.Value);
			Assert.AreEqual(instance.Data.additionalFlag, retrieved.Data.additionalFlag);
		}

		private class ClassWithAnotherClassInside
		{
			internal class InnerClass
			{
				public double Value { get; set; }
			}

			internal class InnerDerivedClass : InnerClass
			{
				internal bool additionalFlag;
			}

			public int Number { get; set; }

			public InnerDerivedClass Data;
		}

		//n-crunch: no coverage start
		[Test, Category("Slow")]
		public void ContentDataCanAlwaysBeLoadedAndSavedViaNames()
		{
			Directory.CreateDirectory("Content");
			using (var textWriter = File.CreateText("Content\\Test.xml"))
				textWriter.WriteLine("<Test></Test>");
			new FileContentLoader(new ContentDataResolver());
			var content = ContentLoader.Load<XmlContent>("Test");
			var data = SaveDataIntoMemoryStream(content);
			Assert.AreEqual("Test".Length + 1, data.Length);
			var loadedContent = LoadDataFromMemoryStream<XmlContent>(data);
			Assert.AreEqual(content.Name, loadedContent.Name);
			Assert.AreEqual(content, loadedContent);
		}
		
		[Test, Category("Slow")]
		public void ThrowExceptionTypeNameStartsWithXml()
		{
			Assert.Throws<NotSupportedException>(() => new XmlBinaryData("Xml").SaveToMemoryStream());
			Assert.AreEqual("Xml", new XmlBinaryData("Xml").Text);
		}

		public class XmlBinaryData
		{
			public XmlBinaryData(string text)
				: this()
			{
				Text = text;
			}

			public string Text { get; private set; }

			private XmlBinaryData() { }
		}
	}
}