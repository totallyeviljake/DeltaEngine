﻿using System;
using System.Globalization;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class StringExtensionsTests
	{
		[Test]
		public void ConvertFloatToInvariantString()
		{
			Assert.AreEqual("2.5", 2.5f.ToInvariantString());
			Assert.AreNotEqual("3.5", 1.5f.ToInvariantString());
			Assert.AreEqual("01", 1.0f.ToInvariantString("00"));
			Assert.AreEqual("1.23", 1.2345f.ToInvariantString("0.00"));
			Assert.AreNotEqual("1.2345", 1.2345f.ToInvariantString("0.00"));
		}

		[Test]
		public void FromInvariantString()
		{
			Assert.AreEqual(1.0f, "1.0".FromInvariantString(0.0f));
			Assert.AreEqual("abc", "abc".FromInvariantString("abc"));
			Assert.AreEqual(0.0f, "val".FromInvariantString(0.0f));
		}

		[Test]
		public void ToInvariantString()
		{
			Assert.AreEqual("null", StringExtensions.ToInvariantString(null));
			Assert.AreEqual("1", StringExtensions.ToInvariantString((object)1.0f));
			Assert.AreEqual("1.2", StringExtensions.ToInvariantString(1.2));
			Assert.AreEqual("1.4", StringExtensions.ToInvariantString(1.4m));
			Assert.AreEqual("abc", StringExtensions.ToInvariantString("abc"));
		}

		[Test]
		public void MaxStringLength()
		{
			Assert.AreEqual(null, ((string)null).MaxStringLength(4));
			Assert.AreEqual("", "".MaxStringLength(4));
			Assert.AreEqual("abcd", "abcd".MaxStringLength(4));
			Assert.AreEqual("ab..", "abcde".MaxStringLength(4));
			Assert.AreEqual("..", "abcde".MaxStringLength(1));
		}

		[Test]
		public static void SplitAndTrimByChar()
		{
			string[] components = "abc, 123, def".SplitAndTrim(',');
			Assert.AreEqual(components.Length, 3);
			Assert.AreEqual(components[0], "abc");
			Assert.AreEqual(components[1], "123");
			Assert.AreEqual(components[2], "def");
		}

		[Test]
		public static void SplitAndTrimByString()
		{
			string[] components = "3 plus 5 is 8".SplitAndTrim("plus", "is");
			Assert.AreEqual(components.Length, 3);
			Assert.AreEqual(components[0], "3");
			Assert.AreEqual(components[1], "5");
			Assert.AreEqual(components[2], "8");
		}

		[Test]
		public void SplitIntoFloats()
		{
			var stringFloats = new[]
			{ "1.0", "2.0", "0511.580254", Math.PI.ToString(CultureInfo.InvariantCulture) };
			var expectedFloats = new[] { 1.0f, 2.0f, 511.580261f, 3.14159274f };
			float[] floats = stringFloats.SplitIntoFloats();
			CollectionAssert.AreEqual(expectedFloats, floats);
		}

		[Test]
		public void SplitIntoFloatsWithSeparator()
		{
			var stringFloats = "1.0, 2.0, 0511.580254";
			var expectedFloats = new[] { 1.0f, 2.0f, 511.580261f };
			float[] floats = stringFloats.SplitIntoFloats();
			CollectionAssert.AreEqual(expectedFloats, floats);
		}

		[Test]
		public void Compare()
		{
			Assert.IsTrue("AbC1".Compare("aBc1"));
			Assert.IsTrue("1.23".Compare("1.23"));
			Assert.IsFalse("Hello".Compare("World"));
		}

		[Test]
		public void ContainsCaseInsensitive()
		{
			Assert.IsTrue("hallo".ContainsCaseInsensitive("ha"));
			Assert.IsTrue("1.23".ContainsCaseInsensitive("1.2"));
			Assert.IsTrue("Hello".ContainsCaseInsensitive("hel"));
			Assert.IsFalse("Banana".ContainsCaseInsensitive("Apple"));
			Assert.IsFalse(((String)null).ContainsCaseInsensitive("abc"));
		}
	}
}