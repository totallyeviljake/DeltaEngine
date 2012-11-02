﻿using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class SizeTests
	{
		[Test]
		public void SizeZero()
		{
			Assert.AreEqual(new Size(), Size.Zero);
			Assert.AreEqual(new Size(0.0f, 0.0f), Size.Zero);
		}

		[Test]
		public void ChangeSize()
		{
			var size = new Size(1.0f, 1.0f) { Height = 2.1f, Width = 2.1f };
			Assert.AreEqual(2.1f, size.Height);
			Assert.AreEqual(2.1f, size.Width);
		}

		[Test]
		public void SizeOne()
		{
			Assert.AreEqual(new Size(1.0f, 1.0f), Size.One);
		}

		[Test]
		public void SizeHalf()
		{
			Assert.AreEqual(new Size(0.5f, 0.5f), Size.Half);
		}

		[Test]
		public void CreateFromFloat()
		{
			const float Width = 3.51f;
			const float Height = 0.23f;
			var s = new Size(Width, Height);

			Assert.AreEqual(s.Width, Width);
			Assert.AreEqual(s.Height, Height);
		}

		[Test]
		public void Equals()
		{
			var s1 = new Size(1, 2);
			var s2 = new Size(3, 4);

			Assert.AreNotEqual(s1, s2);
			Assert.AreEqual(s1, new Size(1, 2));

			Assert.IsTrue(s1 == new Size(1, 2));
			Assert.IsTrue(s1 != s2);
		}

		[Test]
		public void PlusOperator()
		{
			var s1 = new Size(2, 2);
			var s2 = new Size(3, 3);

			Size plus = s1 + s2;
			Assert.AreEqual(plus, new Size(5f, 5f));
		}

		[Test]
		public void MinusOperator()
		{
			var s1 = new Size(2, 2);
			var s2 = new Size(1, 1);

			Size minus = s1 - s2;
			Assert.AreEqual(minus, new Size(1f, 1f));
		}

		[Test]
		public void MultiplyOperator()
		{
			var s1 = new Size(2, 3);
			var s2 = new Size(4, 5);

			Size multiply = s1 * s2;
			Assert.AreEqual(multiply, new Size(8, 15));
		}

		[Test]
		public void MultiplyByFloat()
		{
			Assert.AreEqual(new Size(5, 10), (new Size(2, 4) * 2.5f));
			Assert.AreEqual(new Size(5, 10), 2.5f * (new Size(2, 4)));
		}

		[Test]
		public void DivideSizeByFloat()
		{
			var s = new Size(4, 5);
			const float F = 2f;

			Size divide = s / F;
			Assert.AreEqual(divide, new Size(2.0f, 2.5f));
		}

		[Test]
		public void DivideFloatBySize()
		{
			const float F = 2f;
			var s = new Size(4, 5);

			Size divide = F / s;
			Assert.AreEqual(divide, new Size(0.5f, 0.4f));
		}

		[Test]
		public void ExplicitCastFromPoint()
		{
			var p = new Point(1, 2);
			var s = new Size(1, 2);

			Size addition = (Size)p + s;

			Assert.AreEqual(new Size(2, 4), addition);
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var first = new Size(1, 2);
			var second = new Size(3, 4);
			var sizeValues = new Dictionary<Size, int> { { first, 1 }, { second, 2 } };

			Assert.IsTrue(sizeValues.ContainsKey(first));
			Assert.IsTrue(sizeValues.ContainsKey(second));
			Assert.IsFalse(sizeValues.ContainsKey(new Size(5, 6)));
		}

		[Test]
		public void SizeToString()
		{
			var s = new Size(2.23f, 3.45f);
			Assert.AreEqual("(2.23, 3.45)", s.ToString());
		}

		[Test]
		public void SizeToStringAndFromString()
		{
			var s = new Size(2.23f, 3.45f);
			string sizeString = s.ToString();
			Assert.AreEqual(s, new Size(sizeString));

			Assert.Throws<Size.InvalidNumberOfComponents>(() => new Size("10"));
			Assert.Throws<Size.InvalidNumberOfComponents>(() => new Size("abc"));
		}

		[Test]
		public void Lerp()
		{
			var size1 = new Size(10, 20);
			var size2 = new Size(20, 30);
			var lerp20 = new Size(12, 22);

			Assert.AreEqual(lerp20, Size.Lerp(size1, size2, 0.2f));
			Assert.AreEqual(size1, Size.Lerp(size1, size2, -1));
			Assert.AreEqual(size2, Size.Lerp(size1, size2, 1.1f));
		}

		[Test]
		public void Aspect()
		{
			var portrait = new Size(0.5f, 1f);
			Size square = Size.One;
			var landscape = new Size(1f, 0.5f);

			Assert.AreEqual(0.5f, portrait.Aspect);
			Assert.AreEqual(1f, square.Aspect);
			Assert.AreEqual(2f, landscape.Aspect);
		}
	}
}