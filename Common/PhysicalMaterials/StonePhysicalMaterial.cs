using Terraria.Audio;
using TerrariaOverhaul.Common.Footsteps;
using TerrariaOverhaul.Core.PhysicalMaterials;
using TerrariaOverhaul.Core.Tags;

namespace TerrariaOverhaul.Common.PhysicalMaterials;

public sealed class StonePhysicalMaterial : PhysicalMaterial, ITileTagAssociated, IFootstepSoundProvider
{
	public ContentSet TileTag { get; } = "StoneFootsteps";
	// Footsteps
	public SoundStyle? FootstepSound { get; } = new($"{nameof(TerrariaOverhaul)}/Assets/Sounds/Footsteps/Stone/Step", 8) {
		Volume = 0.5f,
		PitchVariance = 0.1f,
	};
	public SoundStyle? JumpFootstepSound { get; } = new($"{nameof(TerrariaOverhaul)}/Assets/Sounds/Footsteps/Stone/Jump", 3) {
		Volume = 0.375f,
		PitchVariance = 0.1f,
	};
}
