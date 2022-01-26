using System;

[Serializable]
public class TomatoCropData : CropData
{
	private const int maturityFromNewDays = 6;
	private const int maturityFromPickedDays = 3;
	private const int maxPhase = 4;
	private const int maxYield = 3;

	public override int MaturityFromNewDays => maturityFromNewDays;
	public override int MaturityFromPickedDays => maturityFromPickedDays;
	public override int MaxPhase => maxPhase;
	public override int MaxYield => maxYield;
	public override bool Pickable => true;
	public override bool DestroyOnPick => false;
}
