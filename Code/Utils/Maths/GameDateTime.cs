using Godot;
using System;

[Serializable]
public struct GameDateTime : IComparable<GameDateTime>
{
	public long Day;
	public float TimeOfDaySeconds;
	public float TotalTimeInDaySeconds;

	private static Color ColourDawn = new Color(0.22f, 0.53f, 0.69f, 1f);
	private static Color ColourNoon = new Color(1f, 1f, 1f, 1f);
	private static Color ColourDusk = new Color(0.95f, 0.8f, 0.44f, 1f);
	private static Color ColourMidnight = new Color(0.14f, 0.17f, 0.23f, 1f);

	private const float Dawn_Start = 3;
	private const float Dawn_Full = 5.5f;
	private const float Noon_Start = 6f;
	private const float Noon_Full = 10.5f;
	private const float Dusk_Start = 16f;
	private const float Dusk_Full = 18.5f;
	private const float Midnight_Start = 19.5f;
	private const float Midnight_Full = 21f;


	public GameDateTime(long day, float timeOfDaySeconds, float totalTimeInDaySeconds)
	{
		Day = day;
		TimeOfDaySeconds = timeOfDaySeconds;
		TotalTimeInDaySeconds = totalTimeInDaySeconds;
	}

	public GameDateTime AddDays(int days)
	{
		var day = Day + days;
		return new GameDateTime(day, TimeOfDaySeconds, TotalTimeInDaySeconds);
	}

	public GameDateTime AddTime(float time)
	{
		var day = Day;
		var timeOfDaySeconds = TimeOfDaySeconds + time;

		if (timeOfDaySeconds >= TotalTimeInDaySeconds)
		{
			timeOfDaySeconds -= TotalTimeInDaySeconds;
			day++;
		}

		//Debug.Print($"Time: Day = {day}, TimeOfDaySeconds = {timeOfDaySeconds}, TotalTimeInDaySeconds = {this.TotalTimeInDaySeconds}");
		return new GameDateTime(day, timeOfDaySeconds, TotalTimeInDaySeconds);
	}

	public Color GetTimeOfDayColour()
	{
		float dayLerp = TimeOfDaySeconds / TotalTimeInDaySeconds;
		float dayLerpHours = dayLerp * 24;

		if (dayLerpHours <= Dawn_Start)
		{
			return ColourMidnight;
		}
		else if (dayLerpHours <= Dawn_Full)
		{
			float lerpValue = (dayLerpHours - Dawn_Start) / (Dawn_Full - Dawn_Start);
			return CreateSubLerpForColour(ColourMidnight, ColourDawn, lerpValue);
		}
		else if (dayLerpHours <= Noon_Start)
		{
			return ColourDawn;
		}
		else if (dayLerpHours <= Noon_Full)
		{
			float lerpValue = (dayLerpHours - Noon_Start) / (Noon_Full - Noon_Start);
			return CreateSubLerpForColour(ColourDawn, ColourNoon, lerpValue);
		}
		else if (dayLerpHours <= Dusk_Start)
		{
			return ColourNoon;
		}
		else if (dayLerpHours <= Dusk_Full)
		{
			float lerpValue = (dayLerpHours - Dusk_Start) / (Dusk_Full - Dusk_Start);
			return CreateSubLerpForColour(ColourNoon, ColourDusk, lerpValue);
		}
		else if (dayLerpHours <= Midnight_Start)
		{
			return ColourDusk;
		}
		else if (dayLerpHours <= Midnight_Full)
		{
			float lerpValue = (dayLerpHours - Midnight_Start) / (Midnight_Full - Midnight_Start);
			return CreateSubLerpForColour(ColourDusk, ColourMidnight, lerpValue);
		}
		else if (dayLerpHours <= Dawn_Start + 24)
		{
			return ColourMidnight;
		}

		Debug.Print("Something went wrong with colour interpolation");
		return ColourNoon;
	}

	//private static float GetWrapAroundLerpValue(float timeOfDaySeconds, float totalTimeInDaySeconds)
	//{
	//	var lerp = (timeOfDaySeconds + totalTimeInDaySeconds * wrapAroundOffset) / totalTimeInDaySeconds;
	//	if (lerp > 1f) lerp -= 1;
	//	if (lerp < 0f) lerp += 1;
	//	return lerp;
	//}

	private static Color CreateSubLerpForColour(Color from, Color to, float dayLerp)
	{
		var timeOfDayLerpFactor = Mathf.Clamp(dayLerp, 0, 1);
		//var timeOfDayLerpFactor = Mathf.Pow(dayLerp, curveFactor);
		return from.LinearInterpolate(to, timeOfDayLerpFactor);
		//if (dayLerp < 0.6f)
		//{
		//	return from;
		//}
		//else if (dayLerp >= 0.6f && dayLerp <= 0.8f)
		//{
		//	var lerpValue = (dayLerp - 0.6f) / 0.2f;
		//	return from.LinearInterpolate(to, lerpValue);
		//}
		//else
		//{
		//	return to;
		//}


		//var lerpValue = (dayLerp - 0.5f) * 2;
		//lerpValue = Mathf.Clamp(dayLerp, 0, 1);

		//return from.LinearInterpolate(to, dayLerp);
	}

	public int CompareTo(GameDateTime other)
	{
		if (Day > other.Day)
			return 1;
		else if (Day < other.Day)
			return -1;
		else
		{
			if (TimeOfDaySeconds > other.TimeOfDaySeconds)
				return 1;
			else if (TimeOfDaySeconds < other.TimeOfDaySeconds)
				return -1;
			else
				return 0;
		}
	}

	public override bool Equals(object obj)
	{
		return obj is GameDateTime time &&
			   Day == time.Day &&
			   TimeOfDaySeconds == time.TimeOfDaySeconds &&
			   TotalTimeInDaySeconds == time.TotalTimeInDaySeconds;
	}

	public override int GetHashCode()
	{
		int hashCode = -2111934535;
		hashCode = hashCode * -1521134295 + Day.GetHashCode();
		hashCode = hashCode * -1521134295 + TimeOfDaySeconds.GetHashCode();
		hashCode = hashCode * -1521134295 + TotalTimeInDaySeconds.GetHashCode();
		return hashCode;
	}

	public static bool operator <(GameDateTime t1, GameDateTime t2)
	{
		return t1.CompareTo(t2) < 0;
	}

	public static bool operator <=(GameDateTime t1, GameDateTime t2)
	{
		return t1.CompareTo(t2) <= 0;
	}

	public static bool operator >(GameDateTime t1, GameDateTime t2)
	{
		return t1.CompareTo(t2) > 0;
	}

	public static bool operator >=(GameDateTime t1, GameDateTime t2)
	{
		return t1.CompareTo(t2) >= 0;
	}

	public static bool operator ==(GameDateTime t1, GameDateTime t2)
	{
		return t1.CompareTo(t2) == 0;
	}

	public static bool operator !=(GameDateTime t1, GameDateTime t2)
	{
		return t1.CompareTo(t2) != 0;
	}

	public override string ToString()
	{
		int minuteFactor = (int)(TotalTimeInDaySeconds / 24);
		int hours = (int)((TimeOfDaySeconds / TotalTimeInDaySeconds) * 24);
		int minutes = (int)(TimeOfDaySeconds % minuteFactor);
		return $"Day {Day} {hours:D2}:{minutes:D2}";
	}
}
