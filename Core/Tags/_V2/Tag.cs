using System.Runtime.InteropServices;

namespace TerrariaOverhaul.Core.Tags;

[StructLayout(LayoutKind.Auto, Size = sizeof(uint))]
public readonly struct Tag<TSpace>
{
	public readonly uint Id;

	public bool IsValid => Id > 0;
	public string Name => Tags<TSpace>.StringFromTag(this);

	internal Tag(uint id)
	{
		Id = id;
	}

	public override bool Equals(object? obj)
		=> obj is Tag<TSpace> tag && tag.Id == Id;

	public override int GetHashCode()
		=> (int)Id;

	public override string ToString()
		=> $"{Id} - '{Name}'";

	public static bool operator ==(Tag<TSpace> a, Tag<TSpace> b)
		=> a.Id == b.Id;

	public static bool operator !=(Tag<TSpace> a, Tag<TSpace> b)
		=> a.Id != b.Id;

	public static implicit operator string(Tag<TSpace> tag)
		=> tag.Name;

	public static implicit operator Tag<TSpace>(string? name)
		=> Tags<TSpace>.TagFromString(name);
}
