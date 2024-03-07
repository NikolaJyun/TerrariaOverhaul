using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hjson;
using Newtonsoft.Json;
using ReLogic.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaOverhaul.Core.Tags;

internal sealed class SetLoadingSystem : ModSystem
{
	private struct JsonSetExtension
	{
		public string[] Sets;
		public Dictionary<string, string[]> LegacySets;
		public Dictionary<string, string[]> Includes;
	}

	public static Dictionary<string, (ContentSetStorageHandle handle, IdDictionary search)> storageMappings = null!;

	public override void Load()
	{
		storageMappings = new() {
			{ "NPCID", (ContentSets.GetStorageHandle<NPCID>(), NPCID.Search) },
			{ "ItemID", (ContentSets.GetStorageHandle<ItemID>(), ItemID.Search) },
			{ "TileID", (ContentSets.GetStorageHandle<TileID>(), TileID.Search) },
			{ "WallID", (ContentSets.GetStorageHandle<WallID>(), WallID.Search) },
			{ "ProjectileID", (ContentSets.GetStorageHandle<ProjectileID>(), ProjectileID.Search) },
		};
		LoadDataFromMod(Mod);
	}

	public static void LoadDataFromMod(Mod mod)
	{
		var assets = mod.GetFileNames();

		foreach (string fullFilePath in assets.Where(t => t.EndsWith(".tags.hjson"))) {
			using var stream = mod.GetFileStream(fullFilePath);
			using var streamReader = new StreamReader(stream);

			string hjsonText = streamReader.ReadToEnd();
			string jsonText = HjsonValue.Parse(hjsonText).ToString(Stringify.Plain);

			RegisterData(fullFilePath, jsonText);
		}
	}

	//private static readonly Regex operationKeyRegex = new(@"(\w+)(?:\.((?:\w+\|?)*))?", RegexOptions.Compiled);
	//private static readonly JsonSerializerSettings jsonSettings = new() { };

	private static (ContentSetStorageHandle, IdDictionary) GetStorageByName(string name)
	{
		if (!storageMappings.TryGetValue(name, out var result)) {
			throw new ArgumentException($"Unknown set storage: '{name}'.");
		}

		return result;
	}

	//TODO: Optimize, maybe write a memory-efficient HJSON parser..
	private static void RegisterData(string fileName, string jsonText)
	{
		var extensions = JsonConvert.DeserializeObject<Dictionary<string, JsonSetExtension>>(jsonText)
			?? throw new InvalidOperationException($"'{fileName}': Failed to deserialize.");

		foreach (var pair in extensions) {
			string setName = pair.Key;
			var extension = pair.Value;

			ContentSet set = ContentSet.Get(setName);

			if (extension.Includes != null) {
				Include(set, extension.Includes);
			}
		}
	}

	private static void Include(ContentSet set, Dictionary<string, string[]> includes)
	{
		int maxEntries = 0;

		foreach (string[] entries in includes.Values) {
			maxEntries = Math.Max(maxEntries, entries.Length);
		}

		Span<int> indices = stackalloc int[includes.Max(p => p.Value.Length)];

		foreach (var includePair in includes) {
			ReadOnlySpan<string> contexts = includePair.Key.Split('|');
			string[] entries = includePair.Value;
			var indicesSlice = indices.Slice(0, entries.Length);

			foreach (string context in contexts) {
				var (storage, search) = GetStorageByName(context);

				for (int i = 0; i < entries.Length; i++) {
					indicesSlice[i] = search.GetId(entries[i]);
				}

				ContentSets.Include(storage, set.Id, indicesSlice);
			}
		}
	}
}
