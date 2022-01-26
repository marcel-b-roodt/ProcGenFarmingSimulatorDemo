using Godot;
using System;

[Serializable]
public abstract class EntityData : IEntityData
{
	[NonSerialized] public ulong ID;
	public string Name;
	public bool CustomName;
	public Vector2 Position;
}
