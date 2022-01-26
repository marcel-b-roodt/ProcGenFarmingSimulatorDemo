using Godot.Collections;
using System;

[Serializable]
public class PlayerStats
{
	public Dictionary<PlayerSkillID, int> Skills = new Dictionary<PlayerSkillID, int>();

	public PlayerStats()
	{
		foreach (PlayerSkillID skill in SkillLookup.Skills.Keys)
		{
			Skills.Add(skill, 0);
		}
	}
}