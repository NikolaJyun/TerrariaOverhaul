using System;
using System.Collections.Generic;
using System.Numerics;

namespace TerrariaOverhaul.Core.Tags;

internal struct StringIdMap
{
	private uint nextId;
	private string[] stringLookup;
	private readonly Dictionary<string, uint> idLookup;

	public readonly uint NextId => nextId;
	public readonly string[] StringLookup => stringLookup;
	public readonly Dictionary<string, uint> IdLookup => idLookup;

	public StringIdMap() : this(capacity: 32) { }

	public StringIdMap(int capacity)
	{
		nextId = 1;
		idLookup = new(capacity, StringComparer.InvariantCultureIgnoreCase);
		stringLookup = new string[capacity];
	}

	public void Clear()
	{
		idLookup.Clear();
		stringLookup = Array.Empty<string>();
		nextId = 1;
	}

	public readonly string StringFromId(uint id)
	{
		if (id == 0 || id >= NextId) {
			throw new ArgumentOutOfRangeException(nameof(id));
		}

		return StringLookup[id];
	}

	public uint IdFromString(string? str)
	{
		if (str == null || str.Length == 0) {
			return default;
		}

		if (!idLookup.TryGetValue(str, out uint id)) {
			id = nextId++;

			Array.Resize(ref stringLookup, (int)BitOperations.RoundUpToPowerOf2(nextId));

			idLookup[str] = id;
			stringLookup[id] = str;
		}

		return id;
	}
}
