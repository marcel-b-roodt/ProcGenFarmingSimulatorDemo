public interface ICrop : ITileEntity, IHarvestInteractable
{
	InventoryCropSeedType CropType { get; }
	int MaturityFromNewDays { get; }
	int MaturityFromPickedDays { get; }
	int MaxPhase { get; }
	int MaxYield { get; }
	bool Pickable { get; }
	bool DestroyOnPick { get; }
	GameDateTime TimePlanted { get; }
	GameDateTime TimeReadyForHarvest { get; }
	int CurrentPhase { get; }
	bool ReadyToHarvest { get; }

	void BreakCrop();
	void PickCrop();
}

