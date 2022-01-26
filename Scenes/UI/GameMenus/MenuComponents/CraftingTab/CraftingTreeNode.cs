using Godot;
using System;

public abstract class CraftingTreeNode : PanelContainer
{
	public string ItemName;
	public CraftingTreeNode NextNode;
	public CraftingTreeNode PreviousNode;
	public int CategoryPosition;
	public int Index;
	public Action<int> SelectCellAction;

	public void Select()
	{
		AddStyleboxOverride("panel", Themes.Styleboxes.Flat.Highlighted);
	}

	public void Deselect()
	{
		AddStyleboxOverride("panel", Themes.Styleboxes.Flat.Unhighlighted);
	}

	//TODO: Implement a scrolling up and down system the same as the Inventory Tab.
	//Holding the up or down key will start speed scrolling. Wait a delay before doing so
	//Tapping left or right (only once - no repeat factor) will open or close a category, and go up to the higher category if you are in a sub-level. Similar to what already works on the tree

	//TODO: Make the auto-scroll when looking at a long name. 
	//When highlighted, let it slowly scroll to the right to show the name. 
	//Reset the scrolling when moving away.
}