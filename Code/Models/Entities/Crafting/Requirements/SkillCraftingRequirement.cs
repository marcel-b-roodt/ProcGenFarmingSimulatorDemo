public class SkillCraftingRequirement : BaseCraftingRequirement
{
	public PlayerSkillID SkillID;
	public int Level;

	public SkillCraftingRequirement(PlayerSkillID skillID, int level)
	{
		SkillID = skillID;
		Level = level;
	}

	public override bool ValidateRequirement()
	{
		var playerSkill = Global.Instance.PlayerData.Stats.Skills[SkillID];
		return playerSkill >= Level;
	}
}