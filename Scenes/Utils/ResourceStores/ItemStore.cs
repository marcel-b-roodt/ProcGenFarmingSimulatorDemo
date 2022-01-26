using Godot;
using System.Collections.Generic;
using System.Linq;

public static class ItemStore
{
	public static void SpawnItemFromMap(Vector2 position, Dictionary<ItemMetadataID, int> itemDropMap)
	{
		var roll = GameWorld.RNG.RandiRange(0, 100);
		var possibleOutputs = itemDropMap.Where(kvp => roll >= (100 - Mathf.Clamp(kvp.Value, 0, 100))).ToArray();
		var possibleOutputCount = possibleOutputs.Count();

		if (possibleOutputCount > 0)
		{
			var outputIndex = (int)GD.RandRange(0, possibleOutputCount - 1);
			var output = possibleOutputs[outputIndex];
			var item = new InventoryItem(output.Key);

			var targetPosition = PlayerUtils.GetRandomPointNearPosition(position, distance: 40f, onPerimeter: true);
			Global.Instance.GameWorld.SpawnPickup(targetPosition, item, 1);
		}
	}

	public static void SpawnAllItemsFromMap(Vector2 position, Dictionary<ItemMetadataID, int> itemDropMap)
	{
		var roll = GD.RandRange(0, 100);
		var successfulOutputs = itemDropMap.Where(kvp => roll >= (100 - Mathf.Clamp(kvp.Value, 0, 100))).ToArray();

		foreach (var output in successfulOutputs)
		{
			var item = new InventoryItem(output.Key);
			var targetPosition = PlayerUtils.GetRandomPointNearPosition(position, distance: 40f, onPerimeter: true);
			Global.Instance.GameWorld.SpawnPickup(targetPosition, item, 1);

		}
	}
}
