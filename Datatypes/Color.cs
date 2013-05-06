using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Color with a byte per component (red, green, blue, alpha), also provides float properties.
	/// </summary>
	[DebuggerDisplay("Color(R={R}, G={G}, B={B}, A={A})")]
	public struct Color : IEquatable<Color>
	{
		public Color(byte r, byte g, byte b, byte a = 255)
			: this()
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public byte A { get; set; }

		public Color(float r, float g, float b, float a = 1.0f)
			: this()
		{
			R = (byte)(r.Clamp(0.0f, 1.0f) * 255);
			G = (byte)(g.Clamp(0.0f, 1.0f) * 255);
			B = (byte)(b.Clamp(0.0f, 1.0f) * 255);
			A = (byte)(a.Clamp(0.0f, 1.0f) * 255);
		}

		public Color(string colorAsString)
			: this()
		{
			string[] components = colorAsString.SplitAndTrim(',', '(', ')');
			if (components.Length != 4)
				throw new InvalidNumberOfComponents();

			R = FindComponentValue(components, "R");
			G = FindComponentValue(components, "G");
			B = FindComponentValue(components, "B");
			A = FindComponentValue(components, "A");
		}

		public class InvalidNumberOfComponents : Exception {}

		private static byte FindComponentValue(IEnumerable<string> components, string color)
		{
			foreach (string component in components)
				if (IsThisTheRightComponent(component, color))
					return ComponentValue(component);

			throw new InvalidNumberOfComponents();
		}

		private static bool IsThisTheRightComponent(string component, string color)
		{
			if (component.Length < 3 || component.IndexOf('=') < 0)
				throw new InvalidNumberOfComponents();

			return component.StartsWith(color + "=");
		}

		private static byte ComponentValue(string component)
		{
			string valueString = component.Substring(component.IndexOf('=') + 1);
			byte value;
			if (byte.TryParse(valueString, out value))
				return value;

			throw new InvalidColorComponentValue(component);
		}

		public class InvalidColorComponentValue : Exception
		{
			public InvalidColorComponentValue(string message)
				: base(message) {}
		}

		public float RedValue
		{
			get { return R / 255.0f; }
		}

		public float GreenValue
		{
			get { return G / 255.0f; }
		}

		public float BlueValue
		{
			get { return B / 255.0f; }
		}

		public float AlphaValue
		{
			get { return A / 255.0f; }
			set { A = (byte)(value.Clamp(0.0f, 1.0f) * 255); }
		}

		public static readonly Color Black = new Color(0, 0, 0);
		public static readonly Color White = new Color(255, 255, 255);
		public static readonly Color TransparentWhite = Transparent(White);
		public static readonly Color Blue = new Color(0, 0, 255);
		public static readonly Color Cyan = new Color(0, 255, 255);
		public static readonly Color Gray = new Color(128, 128, 128);
		public static readonly Color Green = new Color(0, 255, 0);
		public static readonly Color Orange = new Color(255, 165, 0);
		public static readonly Color Pink = new Color(255, 192, 203);
		public static readonly Color Purple = new Color(255, 0, 255);
		public static readonly Color Red = new Color(255, 0, 0);
		public static readonly Color Teal = new Color(0, 128, 128);
		public static readonly Color Yellow = new Color(255, 255, 0);
		public static readonly Color CornflowerBlue = new Color(100, 149, 237);
		public static readonly Color LightBlue = new Color(0.65f, 0.795f, 1f);
		public static readonly Color LightGray = new Color(165, 165, 165);
		public static readonly Color DarkGray = new Color(89, 89, 89);
		public static readonly Color DarkGreen = new Color(0, 100, 0);
		public static readonly Color Gold = new Color(255, 215, 0);
		public static readonly Color PaleGreen = new Color(152, 251, 152);
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Color));

		public static Color Transparent(Color color)
		{
			return new Color(color.R, color.G, color.B, 0);
		}

		/// <summary>
		/// Colors are stored as RGBA byte values and this gives back the usual RGBA format as an
		/// optimized 32 bit value. R takes the first 8 bits, G the next 8 up to A for the last 8 bits.
		/// </summary>
		public int PackedRgba
		{
			get { return R + (G << 8) + (B << 16) + (A << 24); }
		}

		/// <summary>
		/// Similar to PackedRgba, but R and B are switched around, used for loading Windows bitmaps.
		/// </summary>
		public int PackedBgra
		{
			get { return B + (G << 8) + (R << 16) + (A << 24); }
		}

		public static Color Lerp(Color color1, Color color2, float percentage)
		{
			var r = (byte)(MathExtensions.Lerp(color1.R, color2.R, percentage));
			var g = (byte)(MathExtensions.Lerp(color1.G, color2.G, percentage));
			var b = (byte)(MathExtensions.Lerp(color1.B, color2.B, percentage));
			var a = (byte)(MathExtensions.Lerp(color1.A, color2.A, percentage));
			return new Color(r, g, b, a);
		}

		public static Color GetRandomColor(byte lowValue = 16, byte highValue = 255)
		{
			var r = (byte)Randomizer.Current.Get(lowValue, highValue + 1);
			var g = (byte)Randomizer.Current.Get(lowValue, highValue + 1);
			var b = (byte)Randomizer.Current.Get(lowValue, highValue + 1);
			return new Color(r, g, b);
		}

		public static Color GetRandomBrightColor()
		{
			return GetRandomColor(128);
		}

		public static bool operator !=(Color c1, Color c2)
		{
			return c1.Equals(c2) == false;
		}

		public static bool operator ==(Color c1, Color c2)
		{
			return c1.Equals(c2);
		}

		public bool Equals(Color other)
		{
			return PackedRgba == other.PackedRgba;
		}

		public override bool Equals(object other)
		{
			return other is Color ? Equals((Color)other) : base.Equals(other);
		}

		public override int GetHashCode()
		{
			return PackedRgba;
		}

		public static Color operator *(Color c, float multiplier)
		{
			return new Color(c.RedValue * multiplier, c.GreenValue * multiplier,
				c.BlueValue * multiplier, c.AlphaValue * multiplier);
		}

		public static Color operator *(Color c1, Color c2)
		{
			return new Color(c1.RedValue * c2.RedValue, c1.GreenValue * c2.GreenValue,
				c1.BlueValue * c2.BlueValue, c1.AlphaValue * c2.AlphaValue);
		}

		public override string ToString()
		{
			return "(R=" + R + ", G=" + G + ", B=" + B + ", A=" + A + ")";
		}
	}
}