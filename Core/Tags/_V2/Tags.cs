using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Terraria.ModLoader;

#pragma warning disable IDE0004

namespace TerrariaOverhaul.Core.Tags;

public static class Tags<T>
{
	public static string StringFromTag(Tag<T> tag)
		=> TagSystem.GenericStorage<T>.Storage.StringFromId(tag.Id);

	public static Tag<T> TagFromString(string? name)
		=> new(TagSystem.GenericStorage<T>.Storage.IdFromString(name));

	public static Tag<T>[] TagsFromStrings(ReadOnlySpan<string?> nameIdentifiers)
	{
		var results = new Tag<T>[nameIdentifiers.Length];
		ref var storage = ref TagSystem.GenericStorage<T>.Storage;

		for (int i = 0; i < nameIdentifiers.Length; i++) {
			results[i] = new(storage.IdFromString(nameIdentifiers[i]));
		}

		return results;
	}
}

internal sealed class TagSystem : ModSystem
{
	internal static class GenericStorage<T>
	{
		public static StringIdMap Storage = new();

		static GenericStorage()
		{
			lock (storageHandles) {
				storageHandles.Add(GCHandle.Alloc((object)Storage, GCHandleType.Weak));
			}
		}
	}

	private static readonly List<GCHandle> storageHandles = new();

	public override void Unload()
	{
		lock (storageHandles) {
			for (int i = 0; i < storageHandles.Count; i++) {
				if (storageHandles[i].Target is not object target) {
					storageHandles.RemoveAt(i--);
					continue;
				}

				Unsafe.Unbox<StringIdMap>(target).Clear();
			}
		}
	}
}
