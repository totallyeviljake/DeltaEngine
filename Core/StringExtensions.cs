using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides additional and simplified string manipulation methods.
	/// </summary>
	public static class StringExtensions
	{
		public static string ToInvariantString(this float number)
		{
			return number.ToString(NumberFormatInfo.InvariantInfo);
		}

		public static string ToInvariantString(this float number, string format)
		{
			return number.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		public static T Convert<T>(this string value)
		{
			var type = typeof(T);
			if (type == typeof(string))
				return (T)(System.Convert.ToString(value) as object);
			if (type == typeof(int))
				return (T)(System.Convert.ToInt32(value) as object);
			if (type == typeof(double))
				return (T)(System.Convert.ToDouble(value, CultureInfo.InvariantCulture) as object);
			if (type == typeof(float))
				return (T)(System.Convert.ToSingle(value, CultureInfo.InvariantCulture) as object);
			if (type == typeof(bool))
				return (T)(System.Convert.ToBoolean(value) as object);
			if (type == typeof(char))
				return (T)(System.Convert.ToChar(value) as object);
			if (type.IsEnum)
				return (T)Enum.Parse(type, value);
			if (RegisteredConvertCallbacks.ContainsKey(type))
				return (T)RegisteredConvertCallbacks[type](value);
			throw new NotSupportedException("Type " + type + " was not registered for conversion!");
		}

		private static readonly Dictionary<Type, Func<string, object>> RegisteredConvertCallbacks =
			new Dictionary<Type, Func<string, object>>();

		public static void AddConvertTypeCreation(this Type typeToConvert, Func<string, object> conversion)
		{
			if (!RegisteredConvertCallbacks.ContainsKey(typeToConvert))
				RegisteredConvertCallbacks.Add(typeToConvert, conversion);
		}

		public static string ToInvariantString(object someObj)
		{
			if (someObj == null)
				return "null";

			if (someObj is float)
				return ((float)someObj).ToString(NumberFormatInfo.InvariantInfo);

			if (someObj is double)
				return ((double)someObj).ToString(NumberFormatInfo.InvariantInfo);

			if (someObj is decimal)
				return ((decimal)someObj).ToString(NumberFormatInfo.InvariantInfo);

			return someObj.ToString();
		}

		public static float[] SplitIntoFloats(this string value, char[] separators = null)
		{
			if (separators == null)
				separators = new[] { ',', '(', ')' };

			string[] components = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			return SplitIntoFloats(components);
		}

		public static float[] SplitIntoFloats(this string[] components)
		{
			var floats = new float[components.Length];
			for (int i = 0; i < floats.Length; i++)
				floats[i] = components[i].Convert<float>();

			return floats;
		}

		public static string MaxStringLength(this string value, int maxLength)
		{
			if (String.IsNullOrEmpty(value) || value.Length <= maxLength)
				return value;

			if (maxLength < 2)
				maxLength = 2;

			return value.Substring(0, maxLength - 2).TrimEnd() + "..";
		}

		public static string[] SplitAndTrim(this string value, params char[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.None);
			return TrimAndRemoveEmptyElements(components);
		}

		private static string[] TrimAndRemoveEmptyElements(string[] values)
		{
			var nonEmptyElements = new List<string>();
			for (int i = 0; i < values.Length; i++)
			{
				string trimmedElement = values[i].Trim();
				if (trimmedElement.Length > 0)
					nonEmptyElements.Add(trimmedElement);
			}

			return nonEmptyElements.ToArray();
		}

		public static string[] SplitAndTrim(this string value, params string[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.None);
			return TrimAndRemoveEmptyElements(components);
		}

		public static bool Compare(this string value, string other)
		{
			return String.Compare(value, other, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public static bool ContainsCaseInsensitive(this string value, string searchText)
		{
			return value != null &&
				value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsFirstCharacterInLowerCase(this string word)
		{
			if (String.IsNullOrEmpty(word))
				return true;

			char firstChar = word[0];
			return firstChar < 'A' || firstChar > 'Z';
		}

		public static string ConvertFirstCharactertoUpperCase(this string word)
		{
			if (String.IsNullOrEmpty(word) || !word.IsFirstCharacterInLowerCase())
				return word;

			return (char)(word[0] - 32) + word.Substring(1);
		}

		public static byte[] ToByteArray(string text)
		{
			return Encoding.UTF8.GetBytes(text);
		}

		public static string FromByteArray(byte[] byteArray)
		{
			return Encoding.UTF8.GetString(byteArray);
		}
	}
}