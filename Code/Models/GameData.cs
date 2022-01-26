using System;

[Serializable]
public class GameData
{
	public const float MinutesInDay = 0.1f;//10f;

	public GameDateTime GameTime { get; set; }
	//public float TimeOfDay { get; set; } //86 400 msec in a real-time day
	public static readonly float TotalTimeInDaySeconds = GetTimeScaleForMinutesInDay(MinutesInDay);
	//public static readonly long TimeScale = GetTimeScaleForMinutesInDay(10); //This is a conversion for game time to real minutes per day

	private static float GetTimeScaleForMinutesInDay(float targetMinutesForDay)
	{
		var secondsInDay = 86400;
		var minutesInDay = 1440;
		var targetSecondsInDay = secondsInDay / minutesInDay * targetMinutesForDay;

		Debug.Print($"Total target seconds in a day is {targetSecondsInDay}s");
		return targetSecondsInDay;
	}
}
