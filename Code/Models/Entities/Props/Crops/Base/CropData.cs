using System;

[Serializable]
public abstract class CropData : TileEntityData
{
	public override TileEntityType TileEntityType { get { return TileEntityType.Crop; } }

	public abstract int MaturityFromNewDays { get; }
	public abstract int MaturityFromPickedDays { get; }
	public abstract int MaxPhase { get; }
	public abstract int MaxYield { get; }
	public abstract bool Pickable { get; }
	public abstract bool DestroyOnPick { get; }
	public GameDateTime TimePlanted;
	public GameDateTime TimePlantedOrHarvested;
	public GameDateTime TimeReadyForHarvest;
	public int CurrentPhase = 0;

	public bool ReadyForNextPhase()
	{
		if (CurrentPhase == MaxPhase)
			return false;
		else
		{
			if (Global.Instance.GameTime >= TimeReadyForHarvest)
			{
				Debug.Print($"Crop wasn't at max stage but is past Ready for Harvest time.");
				return true;
			}
			else
			{
				var dayChange = ((float)(TimeReadyForHarvest.Day - TimePlantedOrHarvested.Day) * (CurrentPhase + 1)) / (MaxPhase);
				var timeChange = ((float)(TimeReadyForHarvest.TimeOfDaySeconds - TimePlantedOrHarvested.TimeOfDaySeconds) * (CurrentPhase + 1)) / (MaxPhase);
				var newTime = new GameDateTime(TimePlantedOrHarvested.Day + (long)dayChange, TimePlantedOrHarvested.TimeOfDaySeconds + timeChange, TimePlantedOrHarvested.TotalTimeInDaySeconds);
				var success = Global.Instance.GameTime >= newTime;
				if (success)
				{
					Debug.Print($"Ready for next phase. Current time {Global.Instance.GameTime}. Next phase time {newTime}.");
				}
				return success;
			}
		}
	}
}
