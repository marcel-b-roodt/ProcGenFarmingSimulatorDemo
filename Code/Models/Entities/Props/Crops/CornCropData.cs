using System;

[Serializable]
public class CornCropData : CropData
{
	private const int maturityFromNewDays = 4;
	private const int maturityFromPickedDays = 1;
	private const int maxPhase = 4;
	private const int maxYield = 3;

	public override int MaturityFromNewDays => maturityFromNewDays;
	public override int MaturityFromPickedDays => maturityFromPickedDays;
	public override int MaxPhase => maxPhase;
	public override int MaxYield => maxYield;
	public override bool Pickable => true;
	public override bool DestroyOnPick => false;
}
