using Godot;

public class SkillMetadata
{
	public PlayerSkillID SkillID { get; private set; }
	public string SkillImageName { get; private set; }
	public Texture IconTexture
	{
		get
		{
			if (iconTexture == null)
				iconTexture = ImageStore.Get(SkillImageName);

			return iconTexture;
		}
	}
	private Texture iconTexture;

	public string Name { get; private set; }
	public string Description { get; private set; }

	public SkillMetadata(string skillImageName)
	{
		SkillImageName = skillImageName;
	}

	public void SetID(PlayerSkillID skillID)
	{
		this.SkillID = skillID;
		this.Name = Strings.UI.SkillName(skillID);
		this.Description = Strings.UI.SkillDescription(skillID);
	}
}

public enum PlayerSkillID
{
	building,
	farming,
	mining,
	toolworking,
	metalworking,
}