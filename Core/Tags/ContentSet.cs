using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#pragma warning disable IDE0079
#pragma warning disable IDE0044
#pragma warning disable IDE0052

namespace TerrariaOverhaul.Core.Tags;

internal static class GenericData<T>
{
	public static uint Id;

	static GenericData()
	{
		RuntimeHelpers.RunClassConstructor(typeof(ContentSet).TypeHandle);
	}

	public static void Unregister()
	{
		Id = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void EnsureRegistered()
	{
		if (Id == 0) {
			throw new InvalidOperationException($"'{typeof(T).Name}' is not a registered set storage.");
		}
	}
}

internal struct SetData
{
	[Flags]
	public enum Value : byte
	{
		Default = 0,
		Current = 1,
		Included = 2,
		Excluded = 4,
	}

	public bool[] Values = Array.Empty<bool>();
	//public bool[] ManualInclusions = Array.Empty<bool>();
	//public bool[] ManualExclusions = Array.Empty<bool>();
	public HashSet<bool[]> SetInclusions = new();
	public HashSet<bool[]> SetExclusions = new();

	public SetData() { }

	public static void EnsureLength(ref SetData self, int length)
	{
		Array.Resize(ref self.Values, Math.Max(length, self.Values.Length));
	}
}

internal struct ContentSetStorageData
{
	public SetData[] SetData;
	public Func<int>? GetCount;

	public static ref SetData GetMutableData(ref ContentSetStorageData self, uint index)
	{
		var oldData = self.SetData;
		int oldLength = self.SetData.Length;

		if (index >= self.SetData.Length) {
			int newLength = (int)BitOperations.RoundUpToPowerOf2(ContentSets.Count);
			var newData = new SetData[newLength];

			Array.Copy(oldData, newData, oldLength);
			
			for (int i = oldLength; i < newLength; i++) {
				newData[i] = new();
			}

			self.SetData = newData;
		}

		return ref self.SetData[index];
	}
}

public readonly struct ContentSetStorageHandle
{
	internal readonly uint Id;

	public bool IsValid => Id != 0;

	internal ContentSetStorageHandle(uint id)
	{
		Id = id;
	}
}

internal static class ContentSets
{
	private static StringIdMap stringIdMap = new();
	private static ContentSetStorageData[] storages = new ContentSetStorageData[4];
	private static uint nextStorageId = 1;

	internal static uint Count => stringIdMap.NextId;

	static ContentSets()
	{
		System.IO.File.Copy(
		RegisterStorage<NPCID>(static () => NPCLoader.NPCCount);
		RegisterStorage<ItemID>(static () => ItemLoader.ItemCount);
		RegisterStorage<TileID>(static () => TileLoader.TileCount);
		RegisterStorage<WallID>(static () => WallLoader.WallCount);
		RegisterStorage<ProjectileID>(static () => ProjectileLoader.ProjectileCount);
	}

	public static ContentSet Get(string identifier)
		=> new(stringIdMap.IdFromString(identifier));

	public static ContentSetStorageHandle GetStorageHandle<T>()
	{
		GenericData<T>.EnsureRegistered();
		return new(GenericData<T>.Id);
	}

	public static uint RegisterStorage<T>(Func<int> countGetter)
	{
		uint id = nextStorageId++;
		GenericData<T>.Id = id;

		Array.Resize(ref storages, (int)BitOperations.RoundUpToPowerOf2(id + 1));

		ContentSetStorageData data;
		data.GetCount = countGetter;
		data.SetData = Array.Empty<SetData>();
		storages[id] = data;

		return id;
	}

	public static bool Has(ContentSetStorageHandle storage, uint setId, int entryId)
	{
		var setDataArray = storages[storage.Id].SetData;

		if (setId >= setDataArray.Length) {
			return false;
		}

		bool[] values = setDataArray[setId].Values;

		return entryId < values.Length && values[entryId];
	}

	public static ReadOnlySpan<bool> GetEntries(ContentSetStorageHandle storage, uint setId)
	{
		var setDataArray = storages[storage.Id].SetData;

		return setId < setDataArray.Length ? setDataArray[setId].Values : default;
	}

	public static void Include(ContentSetStorageHandle storage, uint setId, ReadOnlySpan<int> indices)
	{
		ref var data = ref storages[storage.Id];
		ref var setData = ref ContentSetStorageData.GetMutableData(ref data, setId);

		int minLength = data.GetCount!();
		SetData.EnsureLength(ref setData, minLength);

		bool[] values = setData.Values;

		for (int i = 0; i < indices.Length; i++) {
			values[indices[i]] = true;
		}
	}

	public static void Exclude(ContentSetStorageHandle storage, uint setId, ReadOnlySpan<int> indices)
	{
		ref var data = ref storages[storage.Id];
		ref var setData = ref ContentSetStorageData.GetMutableData(ref data, setId);

		int minLength = data.GetCount!();
		SetData.EnsureLength(ref setData, minLength);

		bool[] values = setData.Values;

		for (int i = 0; i < indices.Length; i++) {
			values[indices[i]] = false;
		}
	}
}

public readonly struct ContentSet
{
	public readonly uint Id;

	internal ContentSet(uint id)
	{
		Id = id;
	}

	public bool Has<TStorage>(int entryId)
		=> ContentSets.Has(ContentSets.GetStorageHandle<TStorage>(), Id, entryId);

	public ReadOnlySpan<bool> GetEntries<TStorage>()
		=> ContentSets.GetEntries(ContentSets.GetStorageHandle<TStorage>(), Id);

	public ContentSet Include<TStorage>(params int[] indices)
		=> Include<TStorage>((ReadOnlySpan<int>)indices);

	public ContentSet Include<TStorage>(ReadOnlySpan<int> indices)
	{
		ContentSets.Include(ContentSets.GetStorageHandle<TStorage>(), Id, indices);
		return this;
	}

	public ContentSet Exclude<TStorage>(params int[] indices)
		=> Exclude<TStorage>((ReadOnlySpan<int>)indices);

	public ContentSet Exclude<TStorage>(ReadOnlySpan<int> indices)
	{
		ContentSets.Exclude(ContentSets.GetStorageHandle<TStorage>(), Id, indices);
		return this;
	}

	public ContentSet AttachSet<TStorage>(ref bool[] set)
	{
		//throw new NotImplementedException();
		return this;
	}

	public ContentSet AttachSet(string setName)
		=> AttachSet(setName);

	public ContentSet AttachSet(ContentSet set)
	{
		//throw new NotImplementedException();
		return this;
	}

	public static ContentSet Get(string identifier)
		=> ContentSets.Get(identifier);

	public static implicit operator ContentSet(string identifier)
		=> ContentSets.Get(identifier);
}

public static class ContentSetExtensions
{
	public static bool Has(this ContentSet set, NPC entity)
		=> set.Has<NPCID>(entity.type);

	public static bool Has(this ContentSet set, Item entity)
		=> set.Has<ItemID>(entity.type);

	public static bool Has(this ContentSet set, Projectile entity)
		=> set.Has<ProjectileID>(entity.type);

	public static bool HasTile(this ContentSet set, Tile tile)
		=> set.Has<TileID>(tile.TileType);

	public static bool HasWall(this ContentSet set, Tile tile)
		=> set.Has<WallID>(tile.WallType);
}

//	public static bool HasTag<T>(this T entity, ref ContentSets tag) where T : Entity
//	{
//		return tag.Has(entity);
//	}

//	public static readonly ContentSet Flammable = ContentTags.GetOrCreate(nameof(Flammable))
//		.AttachSet<TileID>(ref TileID.Sets.Grass)
//		.AttachSet<TileID>(ref TileID.Sets.IsVine)
//		.AttachSet<TileID>(ref TileID.Sets.Leaves)
//		.Include<TileID>(new int[] {
//			TileID.WoodBlock,
//			TileID.SpookyWood,
//		});
