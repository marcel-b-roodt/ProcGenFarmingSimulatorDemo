using System;

[Serializable]
public class TallGrassData : CropData
{
	//TODO: Fix this. Grass should grow on its own maybe?
	public override int MaturityFromNewDays => throw new NotImplementedException();
	public override int MaturityFromPickedDays => throw new NotImplementedException();
	public override int MaxPhase => throw new NotImplementedException();
	public override int MaxYield => throw new NotImplementedException();
	public override bool Pickable => throw new NotImplementedException();
	public override bool DestroyOnPick => throw new NotImplementedException();
}
