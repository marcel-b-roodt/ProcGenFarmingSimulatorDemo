using Godot;
using Godot.Collections;

public static class SoundStore
{
	public static class Sounds
	{
		//TODO: Add different sounds here for different types of effects, e.g. hit stone vs hit wood
		public const string ItemDrop = "res://Resources/Sounds/item_drop.wav";
		public const string ItemHitAxe = "res://Resources/Sounds/item_hit_axe.wav";
		public const string ItemPickup = "res://Resources/Sounds/item_pickup.wav";
		public const string ItemSwing = "res://Resources/Sounds/item_swing.wav";

		public const string ErrorSound = "res://Resources/Sounds/error.wav";
	}

	private static Dictionary<string, AudioStream> resources = new Dictionary<string, AudioStream>
	{
		{ nameof(Sounds.ItemDrop),          ResourceLoader.Load<AudioStream>(Sounds.ItemDrop)},
		{ nameof(Sounds.ItemHitAxe),        ResourceLoader.Load<AudioStream>(Sounds.ItemHitAxe)},
		{ nameof(Sounds.ItemPickup),        ResourceLoader.Load<AudioStream>(Sounds.ItemPickup)},
		{ nameof(Sounds.ItemSwing),         ResourceLoader.Load<AudioStream>(Sounds.ItemSwing)},

		{ nameof(Sounds.ErrorSound),        ResourceLoader.Load<AudioStream>(Sounds.ErrorSound)},
	};

	public static AudioStream Get(SoundType name)
	{
		var hasKey = resources.ContainsKey(name.ToString());
		return hasKey ? resources[name.ToString()] : ErrorSound();
	}

	private static AudioStream ErrorSound()
	{
		return resources[nameof(Sounds.ErrorSound)];
	}
}

public enum SoundType
{
	ItemDrop,
	ItemHitAxe,
	ItemPickup,
	ItemSwing,
}