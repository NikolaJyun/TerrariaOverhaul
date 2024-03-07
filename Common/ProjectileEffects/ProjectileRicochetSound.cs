using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaOverhaul.Core.Tags;

namespace TerrariaOverhaul.Common.ProjectileEffects;

[Autoload(Side = ModSide.Client)]
public class ProjectileRicochetSound : GlobalProjectile
{
	public static readonly SoundStyle RicochetSound = new($"{nameof(TerrariaOverhaul)}/Assets/Sounds/HitEffects/Ricochet", 2) {
		Volume = 0.1f,
	};

	private static readonly ContentSet bulletSet = "Bullet";

	public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
		=> bulletSet.Has(projectile);

	public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
	{
		SoundEngine.PlaySound(RicochetSound, projectile.Center);

		return true;
	}
}
