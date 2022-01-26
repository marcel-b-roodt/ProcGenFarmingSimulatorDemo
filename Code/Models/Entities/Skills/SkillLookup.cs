using System.Collections.Generic;

class SkillLookup
{
	public static Dictionary<PlayerSkillID, SkillMetadata> Skills = new Dictionary<PlayerSkillID, SkillMetadata>()
	{
		//TODO: Rewrite these redundant recipe categories and icon lookups into a central lookup place, i.e. passing MetadataID and getting Category, Name and ID
			{ PlayerSkillID.building,           new SkillMetadata(ImageStore.Skills.Skill_Building)},
			{ PlayerSkillID.farming,            new SkillMetadata(ImageStore.Skills.Skill_Farming)},
			{ PlayerSkillID.metalworking,       new SkillMetadata(ImageStore.Skills.Skill_Metalworking) },
			{ PlayerSkillID.mining,             new SkillMetadata(ImageStore.Skills.Skill_Mining) },
			{ PlayerSkillID.toolworking,        new SkillMetadata(ImageStore.Skills.Skill_Toolworking) },
	};

	static SkillLookup()
	{
		foreach (var kvp in Skills)
		{
			Skills[kvp.Key].SetID(kvp.Key);
		}
	}

	public static SkillMetadata Get(PlayerSkillID skillID)
	{
		return Skills[skillID];
	}
}