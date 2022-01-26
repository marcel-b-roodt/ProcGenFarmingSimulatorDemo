using Godot;
using static Helpers;

public class CraftingRequirementNode : HBoxContainer
{
	public TextureRect RequirementImage;
	public Label RequirementLabel;

	private ResourceCraftingRequirement resourceRequirement;
	private SkillCraftingRequirement skillRequirement;

	private int currentValue;
	private int requiredValue;

	public override void _Ready()
	{
		RequirementImage = GetNode<TextureRect>("RequirementImage");
		RequirementLabel = GetNode<Label>("RequirementLabel");
	}

	public void SetRequirement(BaseCraftingRequirement requirement)
	{
		if (requirement is SkillCraftingRequirement)
		{
			skillRequirement = (SkillCraftingRequirement)requirement;
			var skillMetadata = SkillLookup.Get(skillRequirement.SkillID);
			RequirementImage.Texture = skillMetadata.IconTexture;
			currentValue = Global.Instance.PlayerData.Stats.Skills[skillRequirement.SkillID];
			requiredValue = skillRequirement.Level;
			EventManager.ListenEvent(GlobalEventCodes.Player_SkillUpdated, GD.FuncRef(this, nameof(UpdatePlayerSkillLevelValue)));
		}
		else if (requirement is ResourceCraftingRequirement)
		{
			resourceRequirement = (ResourceCraftingRequirement)requirement;
			var itemMetadata = ItemLookup.Get(resourceRequirement.ItemID);
			RequirementImage.Texture = itemMetadata.IconTexture;
			currentValue = Global.Instance.PlayerData.Inventory.GetItemTotalQuantity(resourceRequirement.ItemID);
			requiredValue = resourceRequirement.Quantity;
			EventManager.ListenEvent(GlobalEventCodes.Player_InventoryCellUpdated, GD.FuncRef(this, nameof(UpdatePlayerInventoryQuantityValue)));
		}
		else
		{
			Debug.Print($"Something went wrong with Crafting Requirement: {this.Name}");
		}

		UpdateText();
	}

	public void CleanupRequirement()
	{
		EventManager.IgnoreEvent(GlobalEventCodes.Player_SkillUpdated, GD.FuncRef(this, nameof(UpdatePlayerSkillLevelValue)));
		EventManager.IgnoreEvent(GlobalEventCodes.Player_InventoryCellUpdated, GD.FuncRef(this, nameof(UpdatePlayerInventoryQuantityValue)));
		this.GetParent().RemoveChild(this);
		QueueFree();
	}

	private void UpdateText()
	{
		RequirementLabel.Text = $"{currentValue} / {requiredValue}";
	}

	private void UpdatePlayerSkillLevelValue(int key)
	{
		Debug.Print($"Updating Player Skill Value.");
		currentValue = Global.Instance.PlayerData.Stats.Skills[skillRequirement.SkillID];
		Debug.Print($"New Level: {currentValue}");
		UpdateText();
	}

	private void UpdatePlayerInventoryQuantityValue(int key)
	{
		Debug.Print($"Updating Player Inventory Value.");
		currentValue = Global.Instance.PlayerData.Inventory.GetItemTotalQuantity(resourceRequirement.ItemID);
		Debug.Print($"New Quantity: {currentValue}");
		UpdateText();
	}
}
