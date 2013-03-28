﻿using System;
using DeltaEngine.Core;

namespace Blocks
{
	/// <summary>
	/// Allows a prefix to be applied to the name of each content item prior to loading.
	/// Games can therefore swap mods/skins simply by switching this prefix.
	/// </summary>
	public class BlocksContent : Content
	{
		public BlocksContent(Resolver resolver, string prefix = "",
			bool doBricksSplitInHalfWhenRowFull = false)
			: base(resolver)
		{
			Prefix = prefix;
			DoBricksSplitInHalfWhenRowFull = doBricksSplitInHalfWhenRowFull;
			AreFiveBrickBlocksAllowed = true;
			DoBlocksStartInARandomColumn = false;
		}

		public string Prefix { get; set; }
		public bool DoBricksSplitInHalfWhenRowFull { get; set; }
		public bool AreFiveBrickBlocksAllowed { get; set; }
		public bool DoBlocksStartInARandomColumn { get; set; }

		public new ContentType Load<ContentType>(string contentName) where ContentType : ContentData
		{
			return base.Load<ContentType>(Prefix + contentName);
		}

		public string GetFilenameWithoutPrefix(string filenameWithPrefix)
		{
			if (!filenameWithPrefix.StartsWith(Prefix))
				throw new FilenameWrongPrefixException();

			return filenameWithPrefix.Substring(Prefix.Length);
		}

		public class FilenameWrongPrefixException : Exception {}
	}
}