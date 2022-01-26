using System;

[Serializable]
public class PlayerData : ActorData
{
	public PlayerInventory Inventory { get; private set; }
	public PlayerStats Stats { get; private set; }

	//public float ItemCollectionRange { get; set; }

	public void InitializeNewPlayerVars(string name)
	{
		Name = name;
		Inventory = new PlayerInventory();
		Stats = new PlayerStats();
	}
}
