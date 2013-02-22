﻿using System.Globalization;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// Holds a name and value text pair
	/// </summary>
	public struct XmlAttribute
	{
		public XmlAttribute(string name, object value)
			: this()
		{
			Name = name;
			Value = value.ToString();
		}

		public string Name { get; set; }
		public string Value { get; set; }

		public XmlAttribute(string name, float value)
			: this()
		{
			Name = name;
			Value = value.ToString(CultureInfo.InvariantCulture);
		}

		public XmlAttribute(string name, double value)
			: this()
		{
			Name = name;
			Value = value.ToString(CultureInfo.InvariantCulture);
		}
	}
}