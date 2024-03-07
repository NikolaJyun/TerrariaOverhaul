using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaOverhaul.Common.Hooks.Items;
using TerrariaOverhaul.Core.Tags;

namespace TerrariaOverhaul.Common.Melee;

public sealed class ItemHitSoundReplacements : GlobalItem, IModifyItemNPCHitSound
{
	public static readonly SoundStyle WoodenHitSound = new($"{nameof(TerrariaOverhaul)}/Assets/Sounds/HitEffects/WoodenHit", 3) {
		Volume = 0.3f,
		PitchVariance = 0.1f,
	};

	private static readonly ContentSet woodenSet = "Wooden";

	void IModifyItemNPCHitSound.ModifyItemNPCHitSound(Item item, Player player, NPC target, ref SoundStyle? customHitSound, ref bool playNPCHitSound)
	{
		if (woodenSet.Has(item)) {
			customHitSound = WoodenHitSound;
		}
	}
}
