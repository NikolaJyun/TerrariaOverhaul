﻿using Terraria;
using Terraria.Audio;
using Terraria.ID;
using TerrariaOverhaul.Common.Recoil;
using TerrariaOverhaul.Core.ItemComponents;
using TerrariaOverhaul.Core.ItemOverhauls;
using TerrariaOverhaul.Core.Tags;
using TerrariaOverhaul.Utilities;

namespace TerrariaOverhaul.Common.Guns;

public class GrenadeLauncher : ItemOverhaul
{
	public static readonly SoundStyle GrenadeLauncherFireSound = new($"{nameof(TerrariaOverhaul)}/Assets/Sounds/Items/Guns/GrenadeLauncher/GrenadeLauncherFire") {
		Volume = 0.15f,
		PitchVariance = 0.2f,
	};

	private static readonly ContentSet rocketSet = "Rocket";

	public override bool ShouldApplyItemOverhaul(Item item)
	{
		if (item.useAmmo != AmmoID.Rocket) {
			return false;
		}

		if (!ContentSampleUtils.TryGetProjectile(item.shoot, out var proj)) {
			return false;
		}

		if (proj.aiStyle != ProjAIStyleID.Explosive || rocketSet.Has(proj)) {
			return false;
		}

		return true;
	}

	public override void SetDefaults(Item item)
	{
		base.SetDefaults(item);

		item.UseSound = GrenadeLauncherFireSound;

		if (!Main.dedServ) {
			item.EnableComponent<ItemAimRecoil>();
		}
	}
}
