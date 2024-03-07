﻿using Terraria;
using Terraria.ModLoader;
using TerrariaOverhaul.Common.Camera;
using TerrariaOverhaul.Core.Tags;

namespace TerrariaOverhaul.Common.ProjectileEffects;

[Autoload(Side = ModSide.Client)]
public sealed class ProjectileScreenShake : GlobalProjectile
{
	private static readonly ContentSet explosiveSet = "Explosive";

	public ScreenShake? ScreenShake { get; set; }

	public override bool InstancePerEntity => true;

	public override void SetDefaults(Projectile projectile)
	{
		if (explosiveSet.Has(projectile)) {
			ScreenShake = new ScreenShake(0.8f, 1.0f) {
				Range = 2048f,
			};
		}
	}

	public override void OnKill(Projectile projectile, int timeLeft)
	{
		if (ScreenShake is ScreenShake shake) {
			ScreenShakeSystem.New(shake, projectile.Center);
		}
	}
}
