using Godot;

public class MainMenu : Panel
{
	public bool InventoryActive { get { return Visible && currentTab == MenuTab.Inventory; } }
	public bool CraftingActive { get { return Visible && currentTab == MenuTab.Crafting; } }
	public bool MapActive { get { return Visible && currentTab == MenuTab.Map; } }
	public bool StatsActive { get { return Visible && currentTab == MenuTab.Stats; } }
	public bool OptionsActive { get { return Visible && currentTab == MenuTab.Options; } }

	enum MenuTab
	{
		Inventory,
		Crafting,
		Map,
		Stats,
		Options,
	}

	private MenuTab currentTab;
	private TabContainer body;
	private TextureButton inventoryButton;
	private TextureButton craftingButton;
	private TextureButton mapButton;
	private TextureButton statsButton;
	private TextureButton optionsButton;

	private InventoryTab inventoryTab;
	private CraftingTab craftingTab;
	private MapTab mapTab;
	//private StatsTab statsTab;
	private OptionsTab optionsTab;

	private SceneTree sceneTree;

	public override void _Ready()
	{
		body = GetNode<TabContainer>("Margin/VB/Body");
		inventoryButton = GetNode<TextureButton>("Margin/VB/Headers/InventoryButton");
		craftingButton = GetNode<TextureButton>("Margin/VB/Headers/CraftingButton");
		mapButton = GetNode<TextureButton>("Margin/VB/Headers/MapButton");
		statsButton = GetNode<TextureButton>("Margin/VB/Headers/StatsButton");
		optionsButton = GetNode<TextureButton>("Margin/VB/Headers/OptionsButton");

		inventoryTab = GetNode<InventoryTab>("Margin/VB/Body/InventoryTab");
		craftingTab = GetNode<CraftingTab>("Margin/VB/Body/CraftingTab");
		mapTab = GetNode<MapTab>("Margin/VB/Body/MapTab");
		//statsTab = GetNode<StatsTab>("Margin/VB/Body/StatsTab");
		optionsTab = GetNode<OptionsTab>("Margin/VB/Body/OptionsTab");

		_on_InventoryButton_pressed();
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Visible)
		{
			if (@event.IsActionPressed(Helpers.PlayerInputCodes.L1) && !sceneTree.IsInputHandled())
			{
				SwitchTabDown();
				sceneTree.SetInputAsHandled();
			}
			else if (@event.IsActionPressed(Helpers.PlayerInputCodes.R1) && !sceneTree.IsInputHandled())
			{
				SwitchTabUp();
				sceneTree.SetInputAsHandled();
			}
		}
	}

	public void ShowCurrentTab()
	{
		switch (currentTab)
		{
			case MenuTab.Inventory:
				inventoryTab.Show();
				break;
			case MenuTab.Crafting:
				craftingTab.Show();
				break;
			case MenuTab.Map:
				mapTab.Show();
				break;
			case MenuTab.Stats:
				//statsTab.Show();
				break;
			case MenuTab.Options:
				optionsTab.Show();
				break;
		}
	}

	private void SwitchTabUp()
	{
		switch (currentTab)
		{
			case MenuTab.Inventory:
				_on_CraftingButton_pressed();
				break;
			case MenuTab.Crafting:
				_on_MapButton_pressed();
				break;
			case MenuTab.Map:
				_on_StatsButton_pressed();
				break;
			case MenuTab.Stats:
				_on_OptionsButton_pressed();
				break;
			case MenuTab.Options:
				_on_InventoryButton_pressed();
				break;
		}
	}

	private void SwitchTabDown()
	{
		switch (currentTab)
		{
			case MenuTab.Inventory:
				_on_OptionsButton_pressed();
				break;
			case MenuTab.Crafting:
				_on_InventoryButton_pressed();
				break;
			case MenuTab.Map:
				_on_CraftingButton_pressed();
				break;
			case MenuTab.Stats:
				_on_MapButton_pressed();
				break;
			case MenuTab.Options:
				_on_StatsButton_pressed();
				break;
		}
	}

	private void ResetTextureOnCurrentHeaderButton()
	{
		switch (currentTab)
		{
			case MenuTab.Inventory:
				inventoryButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				break;
			case MenuTab.Crafting:
				craftingButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				break;
			case MenuTab.Map:
				mapButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				break;
			case MenuTab.Stats:
				statsButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				break;
			case MenuTab.Options:
				optionsButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				break;
		}
	}

	#region Signals
	private void _on_InventoryButton_pressed()
	{
		ResetTextureOnCurrentHeaderButton();
		currentTab = MenuTab.Inventory;
		body.CurrentTab = (int)currentTab;
		inventoryButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
		inventoryTab.Show();
	}

	private void _on_CraftingButton_pressed()
	{
		ResetTextureOnCurrentHeaderButton();
		currentTab = MenuTab.Crafting;
		body.CurrentTab = (int)currentTab;
		craftingButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
		craftingTab.Show();
	}

	private void _on_MapButton_pressed()
	{
		ResetTextureOnCurrentHeaderButton();
		currentTab = MenuTab.Map;
		body.CurrentTab = (int)currentTab;
		mapButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
		mapTab.Show();
	}

	private void _on_StatsButton_pressed()
	{
		ResetTextureOnCurrentHeaderButton();
		currentTab = MenuTab.Stats;
		body.CurrentTab = (int)currentTab;
		statsButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
	}

	private void _on_OptionsButton_pressed()
	{
		ResetTextureOnCurrentHeaderButton();
		currentTab = MenuTab.Options;
		body.CurrentTab = (int)currentTab;
		optionsButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
		optionsTab.Show();
	}
	#endregion
}
