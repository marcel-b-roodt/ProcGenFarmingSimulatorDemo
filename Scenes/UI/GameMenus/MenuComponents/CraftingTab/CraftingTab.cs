using Godot;
using System.Collections.Generic;
using System.Linq;
using static Helpers;

public class CraftingTab : Panel
{
	private const int itemIconSize = 16;

	private MainMenu mainMenu;

	private PanelContainer CraftingTreeContainer;
	private VBoxContainer CraftingTree;
	private PanelContainer SelectedRecipeRequirementsContainer;
	private VBoxContainer SelectedRecipeRequirementsHolder;
	private VBoxContainer SelectedRecipeContainer;
	private TextureRect SelectedRecipeItemDisplayImage;
	private Label SelectedRecipeItemDescriptionLabel;

	private UIInput UIInputSelect;
	private UIInput UIInputCraft;
	private UIInput UIInputBack;

	//private int selectedCategory = -1;
	//private int selectedRecipe = -1;
	private bool recipeConfirmed;

	private CraftingTreeNode selectedNode;
	private int selectedIndex;

	//private bool CategorySelected { get { return selectedCategory >= 0 && selectedRecipe < 0; } }
	//private bool RecipeSelected { get { return selectedRecipe >= 0; } }

	private Dictionary<int, int> categoryNodeIndexes = new Dictionary<int, int>(); //Keep track of where the nodes are in the position of children of the tree
	private Dictionary<int, int> categoryRecipeCount = new Dictionary<int, int>(); //Keep track of all of the recipe counts for the category, so we know how many to hide, mark as unfocused, or jump in indexes between categories
	private Dictionary<int, bool> categoryExpanded = new Dictionary<int, bool>();
	private Dictionary<int, List<CraftingTreeRecipeNode>> categoryToRecipes = new Dictionary<int, List<CraftingTreeRecipeNode>>();

	private SceneTree sceneTree;

	public override void _Ready()
	{
		mainMenu = (MainMenu)FindParent("MainMenu");

		CraftingTreeContainer = GetNode<PanelContainer>("VB/HB/PC_CraftingRecipes");
		CraftingTree = GetNode<VBoxContainer>("VB/HB/PC_CraftingRecipes/SC/VB_Tree");

		SelectedRecipeRequirementsContainer = GetNode<PanelContainer>("VB/HB/PC_SelectedRecipeRequirements");
		SelectedRecipeRequirementsHolder = GetNode<VBoxContainer>("VB/HB/PC_SelectedRecipeRequirements/MC/VB");
		SelectedRecipeContainer = GetNode<VBoxContainer>("VB/HB/VB_SelectedRecipeContainer");
		SelectedRecipeItemDisplayImage = GetNode<TextureRect>("VB/HB/VB_SelectedRecipeContainer/MC/VB_ItemDisplay/VB_ItemImage/ItemImage");
		SelectedRecipeItemDescriptionLabel = GetNode<Label>("VB/HB/VB_SelectedRecipeContainer/MC/VB_ItemDisplay/SC/ItemDescription");

		UIInputSelect = GetNode<UIInput>("VB/HB_Controls/UIInput_Select");
		UIInputCraft = GetNode<UIInput>("VB/HB_Controls/UIInput_Craft");
		UIInputBack = GetNode<UIInput>("VB/HB_Controls/UIInput_Back");

		PopulateTreeWithRecipes();
		SelectedRecipeItemDisplayImage.Texture = null;

		SelectNode(0);
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (mainMenu.CraftingActive)
		{
			if (recipeConfirmed)
			{
				if (Input.IsActionJustPressed(PlayerInputCodes.A) && @event.IsAction(PlayerInputCodes.A) && !sceneTree.IsInputHandled())
				{
					CraftItem();
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.B) && @event.IsAction(PlayerInputCodes.B) && !sceneTree.IsInputHandled())
				{
					DeselectCraftingRecipe();
					sceneTree.SetInputAsHandled();
				}
			}

			if (!recipeConfirmed)
			{

				if (Input.IsActionJustPressed(PlayerInputCodes.A) && @event.IsAction(PlayerInputCodes.A) && !sceneTree.IsInputHandled())
				{
					if (selectedNode is CraftingTreeRecipeNode)
					{
						SelectCraftingRecipe();
						sceneTree.SetInputAsHandled();
					}
					else if (selectedNode is CraftingTreeCategoryNode)
					{
						ToggleCategoryCollapsed();
						sceneTree.SetInputAsHandled();
					}
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Up) && @event.IsAction(PlayerInputCodes.UI_Up) && !sceneTree.IsInputHandled())
				{
					SelectPreviousNode();
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Down) && @event.IsAction(PlayerInputCodes.UI_Down) && !sceneTree.IsInputHandled())
				{
					SelectNextNode();
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Left) && @event.IsAction(PlayerInputCodes.UI_Left) && !sceneTree.IsInputHandled())
				{
					ToggleCategoryCollapsed();
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Right) && @event.IsAction(PlayerInputCodes.UI_Right) && !sceneTree.IsInputHandled())
				{
					ExpandCategory();
					sceneTree.SetInputAsHandled();
				}
			}
		}
	}

	public new void Show()
	{
		base.Show();

		DeselectCraftingRecipe();
		SelectNode(selectedIndex);
	}

	public new void Hide()
	{
		base.Hide();
	}

	private void SelectNextNode()
	{
		CraftingTreeNode nextNode = null;
		CraftingTreeNode indexNode = selectedNode.NextNode;
		while (nextNode == null)
		{
			var nodeSuggestion = indexNode;
			if (nodeSuggestion.Visible)
				nextNode = nodeSuggestion;
			else
				indexNode = indexNode.NextNode;
		}

		SelectNode(nextNode);
	}

	private void SelectPreviousNode()
	{
		CraftingTreeNode previousNode = null;
		CraftingTreeNode indexNode = selectedNode.PreviousNode;
		while (previousNode == null)
		{
			var nodeSuggestion = indexNode;
			if (nodeSuggestion.Visible)
				previousNode = nodeSuggestion;
			else
				indexNode = indexNode.PreviousNode;
		}

		SelectNode(previousNode);
	}

	private void ToggleCategoryCollapsed(int index)
	{
		SelectNode(index);
		ToggleCategoryCollapsed();
	}

	private void ToggleCategoryCollapsed()
	{
		var currentCategory = selectedNode.CategoryPosition;
		var currentCategoryExpanded = categoryExpanded[currentCategory];
		if (currentCategoryExpanded)
		{
			categoryExpanded[currentCategory] = false;
			UpdateRecipeVisibilityForCategory(currentCategory, false);
			SelectNode(categoryNodeIndexes[currentCategory]);
			CraftingTree.GetChild<CraftingTreeCategoryNode>(categoryNodeIndexes[currentCategory]).ToggleCategoryAccordion(false);
		}
		else
		{
			categoryExpanded[currentCategory] = true;
			UpdateRecipeVisibilityForCategory(currentCategory, true);
			CraftingTree.GetChild<CraftingTreeCategoryNode>(categoryNodeIndexes[currentCategory]).ToggleCategoryAccordion(true);
		}
	}

	private void ExpandCategory()
	{
		var currentCategory = selectedNode.CategoryPosition;
		var currentCategoryExpanded = categoryExpanded[currentCategory];
		if (!currentCategoryExpanded)
		{
			categoryExpanded[currentCategory] = true;
			UpdateRecipeVisibilityForCategory(currentCategory, true);
			CraftingTree.GetChild<CraftingTreeCategoryNode>(categoryNodeIndexes[currentCategory]).ToggleCategoryAccordion(true);

		}
	}

	private void UpdateRecipeVisibilityForCategory(int selectedCategory, bool visible)
	{
		var categoryRootIndex = categoryNodeIndexes[selectedCategory];
		var categoryFirstChild = categoryRootIndex + 1;
		var categoryLastChildIndex = categoryRootIndex + categoryRecipeCount[selectedCategory];
		for (int i = categoryFirstChild; i <= categoryLastChildIndex; i++)
		{
			var recipeNode = CraftingTree.GetChild<CraftingTreeRecipeNode>(i);
			recipeNode.Visible = visible;
			recipeNode.FocusMode = visible ? FocusModeEnum.All : FocusModeEnum.None;
		}
	}

	private void SelectNode(int index)
	{
		var node = CraftingTree.GetChild<CraftingTreeNode>(index);
		SelectNode(node);
	}

	private void SelectNode(CraftingTreeNode node)
	{
		selectedNode?.Deselect();
		selectedNode = node;
		UpdateCraftingRecipeImage();
		selectedNode.Select();
		selectedNode.GrabFocus();
		//selectedNode.GrabFocus();
	}

	private void CraftItem()
	{
		if (!(selectedNode is CraftingTreeRecipeNode))
			return;

		bool validatedRecipe = true;
		var recipeMetadataID = (selectedNode as CraftingTreeRecipeNode).ItemMetadataID.Value;
		var craftingMetadata = CraftingLookup.GetItemByMetadata(recipeMetadataID);

		if (craftingMetadata.Requirements.Count() == 0)
		{
			validatedRecipe = false;
		}

		foreach (var requirement in craftingMetadata.Requirements)
		{
			if (!requirement.ValidateRequirement())
				validatedRecipe = false;
		}

		if (validatedRecipe)
		{
			foreach (var requirement in craftingMetadata.Requirements)
			{
				if (requirement is ResourceCraftingRequirement)
				{
					var resourceRequirement = (ResourceCraftingRequirement)requirement;
					Global.Instance.PlayerData.Inventory.ConsumeItem(resourceRequirement.ItemID, resourceRequirement.Quantity);
				}
			}

			Global.Instance.PlayerData.Inventory.AddItem(new InventoryItem(craftingMetadata.OutputID), 1, out _);
		}
		else
		{
			Debug.Print($"Lacking a resource or skill to craft!");
		}
	}

	private void SelectCraftingRecipe()
	{
		if ((selectedNode is CraftingTreeRecipeNode) && !recipeConfirmed)
		{
			recipeConfirmed = true;
			CraftingTreeContainer.Visible = false;
			//CraftingTree.FocusMode = FocusModeEnum.None; //TODO: Store all of the focus types and turn them off;
			//CraftingTree.ReleaseFocus();
			//SelectedRecipeContainer.Visible = true;
			var recipeMetadataID = (selectedNode as CraftingTreeRecipeNode).ItemMetadataID.Value;
			var recipeItemMetadata = ItemLookup.Get(recipeMetadataID);
			LoadCraftingRecipeRequirements(recipeMetadataID);
			SelectedRecipeContainer.SizeFlagsStretchRatio = 8;
			//SelectedRecipeItemDisplayImage.Texture = recipeItemMetadata.IconTexture;
			SelectedRecipeItemDescriptionLabel.Text = recipeItemMetadata.Description;
			UIInputSelect.Visible = false;
			UIInputCraft.Visible = true;
			UIInputBack.Visible = true;
		}
	}

	private void DeselectCraftingRecipe()
	{
		if (recipeConfirmed)
		{
			recipeConfirmed = false;
			CraftingTreeContainer.Visible = true;
			//CraftingTree.FocusMode = FocusModeEnum.All; //TODO: Restore all of the focus types.
			//SelectNode(recipeLeaves[selectedCategory].Values.ToList()[selectedRecipe]);
			//CraftingTree.GrabFocus();
			//SelectedRecipeContainer.Visible = false;
			UnloadCraftingRecipeRequirements();
			SelectedRecipeContainer.SizeFlagsStretchRatio = 1;
			UIInputSelect.Visible = true;
			UIInputCraft.Visible = false;
			UIInputBack.Visible = false;
		}
	}

	private void LoadCraftingRecipeRequirements(ItemMetadataID metadataID)
	{
		SelectedRecipeRequirementsContainer.Visible = true;
		var craftingMetadata = CraftingLookup.GetItemByMetadata(metadataID);
		foreach (var requirement in craftingMetadata.Requirements)
		{
			var requirementNode = SceneStore.Instantiate<CraftingRequirementNode>();
			SelectedRecipeRequirementsHolder.AddChild(requirementNode);
			requirementNode.SetRequirement(requirement);
		}
	}

	private void UnloadCraftingRecipeRequirements()
	{
		SelectedRecipeRequirementsContainer.Visible = false;
		foreach (Node child in SelectedRecipeRequirementsHolder.GetChildren())
		{
			if (child is CraftingRequirementNode)
			{
				var requirementChild = (CraftingRequirementNode)child;
				requirementChild.CleanupRequirement();
			}
		}
	}

	private void PopulateTreeWithRecipes()
	{
		int index = 0;
		int categoryPosition = 0;
		CraftingTreeNode previousNode = null;

		foreach (var categoryDictionaryKvp in CraftingLookup.Recipes)
		{
			var categoryKey = categoryDictionaryKvp.Key;

			var categoryLeaf = SceneStore.Instantiate<CraftingTreeCategoryNode>();
			categoryLeaf.SelectCellAction = SelectCell;
			categoryLeaf.ItemName = Strings.UI.CraftingCategory(categoryKey);
			categoryLeaf.CategoryPosition = categoryPosition;
			categoryLeaf.Index = index;

			CraftingTree.AddChild(categoryLeaf);
			categoryLeaf.ConfigureExpanderClick(this, nameof(ToggleCategoryCollapsed));
			categoryLeaf.ConfigureSelectClick(this, nameof(SelectCell));

			if (previousNode != null)
			{
				previousNode.NextNode = categoryLeaf;
				categoryLeaf.PreviousNode = previousNode;
			}
			previousNode = categoryLeaf;

			categoryNodeIndexes.Add(categoryPosition, index);
			categoryExpanded.Add(categoryPosition, true);
			categoryToRecipes.Add(categoryPosition, new List<CraftingTreeRecipeNode>());
			index++;

			int recipePosition = 0;

			foreach (var kvp in categoryDictionaryKvp.Value)
			{
				var itemMetadata = ItemLookup.Get(kvp.Key);

				var recipeLeaf = SceneStore.Instantiate<CraftingTreeRecipeNode>();
				recipeLeaf.SelectCellAction = SelectCell;
				recipeLeaf.CategoryNode = categoryLeaf;
				recipeLeaf.ItemIcon = itemMetadata.IconTexture;
				recipeLeaf.ItemName = itemMetadata.Name;
				recipeLeaf.ItemMetadataID = itemMetadata.ID;
				recipeLeaf.CategoryPosition = categoryPosition;
				recipeLeaf.RecipePosition = recipePosition;
				recipeLeaf.Index = index;

				CraftingTree.AddChild(recipeLeaf);
				recipeLeaf.ConfigureSelectClick(this, nameof(SelectCell));

				if (previousNode != null)
				{
					previousNode.NextNode = recipeLeaf;
					recipeLeaf.PreviousNode = previousNode;
				}
				previousNode = recipeLeaf;

				categoryToRecipes[categoryPosition].Add(recipeLeaf);
				index++;
				recipePosition++;
			}

			categoryRecipeCount.Add(categoryPosition, recipePosition);
			categoryPosition++;

			var firstNode = CraftingTree.GetChild<CraftingTreeNode>(0);
			firstNode.PreviousNode = previousNode;
			previousNode.NextNode = firstNode;
		}
	}

	private void UpdateCraftingRecipeImage()
	{
		if (selectedNode is CraftingTreeRecipeNode)
		{
			var recipeMetadataID = (selectedNode as CraftingTreeRecipeNode).ItemMetadataID.Value;
			var recipeItemMetadata = ItemLookup.Get(recipeMetadataID);
			SelectedRecipeItemDisplayImage.Texture = recipeItemMetadata.IconTexture;
			SelectedRecipeItemDescriptionLabel.Text = recipeItemMetadata.Description;
		}
		else
		{
			SelectedRecipeItemDisplayImage.Texture = null;
			SelectedRecipeItemDescriptionLabel.Text = "";
		}
	}

	private void SelectCell(int index)
	{
		//Debug.Print($"Selecting Cell: {index}");
		SelectNode(index);
	}
}


