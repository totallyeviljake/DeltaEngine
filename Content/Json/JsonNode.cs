using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeltaEngine.Content.Json
{
	/// <summary>
	/// Provides json text parsing support without having to include the Newtonsoft Json yourself.
	/// </summary>
	public class JsonNode
	{
		public JsonNode(string text)
		{
			if (String.IsNullOrEmpty(text))
				throw new NeedValidText();

			data = JObject.Parse(text);
		}

		private JsonNode(JToken parsedJsonData)
		{
			data = parsedJsonData;
		}

		public class NeedValidText : Exception {}

		private readonly JToken data;

		public int NumberOfNodes
		{
			get { return ((JContainer)data).Count; }
		}

		public T Get<T>(string nodeName)
		{
			var node = data[nodeName];
			if (node == null)
				throw new NodeNotFound(nodeName);

			return node.Value<T>();
		}

		public class NodeNotFound : Exception
		{
			public NodeNotFound(string nodeName)
				: base("Node not found: " + nodeName) {}
		}

		public JsonNode this[string childName]
		{
			get { return new JsonNode(data[childName]); }
		}

		public JsonNode this[int arrayIndex]
		{
			get
			{
				if (data is JArray)
					return FindJsonArrayElement(arrayIndex);

				return new JsonNode(data[arrayIndex]);
			}
		}

		private JsonNode FindJsonArrayElement(int arrayIndex)
		{
			int counter = 0;
			foreach (var element in data)
				if (counter++ == arrayIndex)
					return new JsonNode(element);

			throw new IndexOutOfRangeException();
		}

		public int[] GetIntArray()
		{
			var array = data as JArray;
			return array.Values<int>().ToArray();
		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject(data);
		}
	}
}