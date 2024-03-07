﻿using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaOverhaul.Common.Footsteps;
using TerrariaOverhaul.Core.PhysicalMaterials;
using TerrariaOverhaul.Core.Tags;

namespace TerrariaOverhaul.Common.PhysicalMaterials;

public sealed class GrassPhysicalMaterial : PhysicalMaterial, ITileTagAssociated, IFootstepSoundProvider
{
	public ContentSet TileTag { get; } = "GrassFootsteps";

	// Footsteps
	public SoundStyle? FootstepSound { get; } = new($"{nameof(TerrariaOverhaul)}/Assets/Sounds/Footsteps/Grass/Step", 8) {
		Volume = 0.5f,
		PitchVariance = 0.1f,
	};

	public SoundStyle? JumpFootstepSound => ModContent.GetInstance<DirtPhysicalMaterial>().JumpFootstepSound;
}
