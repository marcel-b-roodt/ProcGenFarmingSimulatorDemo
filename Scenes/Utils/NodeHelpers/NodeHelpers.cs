using Godot;

public static class NodeHelpers
{
	public static void InitialiseEntity(IEntity entity, EntityData data)
	{
		if (!(entity is PlayerEntity) &&
			!(entity is ItemPickupEntity))
			SetupNameTagHolder(entity, data);
	}

	public static void SetupNameTagHolder(IEntity entity, EntityData data)
	{
		var entityNode = (Node2D)entity;
		entity.NameTagHolder = entityNode.GetNode<NameTagHolder>("NameTagHolder");

		if (data != null && data.CustomName)
		{
			entity.NameTagHolder.SetupName(data.Name, false);
		}
		else
		{
			entity.NameTagHolder.HideName();
		}
	}

	public static void NameNode(IEntity entity, EntityData data, string name)
	{
		data.Name = name;
		data.CustomName = true;
		entity.NameTagHolder.SetupName(name, true);
	}
}
