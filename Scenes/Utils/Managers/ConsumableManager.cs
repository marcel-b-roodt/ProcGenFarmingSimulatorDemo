using Godot;
using System.Collections.Generic;

public static class ConsumableManager
{
	private static EntityContainer entityContainer;

	#region Items
	public static void UseConsumableItem(IEntity entity, PlayerInventorySlot activeItem, List<object> areasAndBodies)
	{
		var consumableMetadata = (ConsumableItemMetadata)activeItem.Item.Metadata;
		Debug.Print($"Consumable metadata is {consumableMetadata.Type}; {consumableMetadata.ConsumableType}");

		if (consumableMetadata is FoodConsumableMetadata)
		{

		}
		else if (consumableMetadata is NameTagConsumableMetadata)
		{
			entityContainer = CreateEntityContainer(NodeUtils.GetClosestNode<IEntity>(entity.Position, areasAndBodies));
			Debug.Print($"Attempting to rename {entityContainer.EntityData.Name ?? entityContainer.Entity.Name}.");
			GenericInputDialogueMenu.WaitForDialogue(Strings.UI.Static.GenericInputDialogueTitle, Strings.UI.Static.GenericInputDialogueText, entityContainer.EntityData.Name, ConfirmNameChangeForObject, CancelNameChangeForObject, activeItem);
		}
		else if (consumableMetadata is PotionConsumableMetadata)
		{

		}
	}
	#endregion

	#region NameTagFunctions
	private static void ConfirmNameChangeForObject(string name, object[] inputArgs)
	{
		var activeItem = (PlayerInventorySlot)inputArgs[0];

		var previousName = entityContainer.EntityData.Name;
		var previousQuantity = activeItem.Quantity;
		NodeHelpers.NameNode(entityContainer.Entity, entityContainer.EntityData, name);
		activeItem.ConsumeItem();

		Debug.Print($"Renamed entity {previousName} to {entityContainer.EntityData.Name}. Consumed {activeItem.Item.Metadata.Name}. Quantity went from {previousQuantity} to {activeItem.Quantity}.");
		entityContainer = null;
	}

	private static void CancelNameChangeForObject()
	{
		entityContainer = null;
	}
	#endregion

	private class EntityContainer
	{
		public IEntity Entity;
		public Node2D EntityNode;
		public EntityData EntityData;
	}

	private static EntityContainer CreateEntityContainer(Node2D node)
	{
		return new EntityContainer
		{
			Entity = (IEntity)node,
			EntityNode = node,
			EntityData = ((EntityData)((IEntity)node).Data)
		};
	}
}
