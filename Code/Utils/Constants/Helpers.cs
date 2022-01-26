
public static class Helpers
{
	public static class CollisionLayers
	{
		//Note: Layers are exactly that. The layer it's on
		//Masks are the layers that it should collide with
		public static int GeneralLayer = 1;
		public static int ItemPickupLayer = 2;
		public static int MetaLayer = 4;
	}

	public static class GlobalEventCodes
	{
		public static string Debug_Toggle = "Debug_Toggle";
		public static string Debug_ToggleMenu = "Debug_ToggleMenu";

		public static string Dialogue_Complete = "Dialogue_Complete";

		public static string Game_DayUpdated = "Game_DayUpdated";

		public static string Global_PlayerInputUpdated = "Global_PlayerInputUpdated";

		public static string Player_InventoryCellUpdated = "Player_InventoryCellUpdated";
		public static string Player_InventoryBackpackUpgraded = "Player_InventoryBackpackUpgraded";
		public static string Player_InventoryQuickSlotUpdated = "Player_InventoryQuickSlotUpdated";
		public static string Player_SkillUpdated = "Player_SkillUpdated";

		public static string World_CellUpdated = "World_CellUpdated";
	}

	public static class PlayerInputCodes
	{
		public static string UI_Accept = "ui_accept";
		public static string UI_Select = "ui_select";
		public static string UI_Cancel = "ui_cancel";
		public static string UI_Focus_Next = "ui_focus_next";
		public static string UI_Focus_Prev = "ui_focus_prev";
		public static string UI_Up = "ui_up";
		public static string UI_Down = "ui_down";
		public static string UI_Left = "ui_left";
		public static string UI_Right = "ui_right";

		public static string Up = "up";
		public static string Down = "down";
		public static string Left = "left";
		public static string Right = "right";

		public static string DPad_Up = "dpad_up";
		public static string DPad_Down = "dpad_down";
		public static string DPad_Left = "dpad_left";
		public static string DPad_Right = "dpad_right";

		public static string A = "action_a";
		public static string B = "action_b";
		public static string X = "action_x";
		public static string Y = "action_y";

		public static string L1 = "action_l1";
		public static string L2 = "action_l2";
		public static string L3 = "action_l3";
		public static string R1 = "action_r1";
		public static string R2 = "action_r2";
		public static string R3 = "action_r3";

		public static string Start = "action_start";
		public static string Select = "action_select";
	}

	public static class SaveSlots
	{
		public static string SavePath(SaveSlot slot)
		{
			switch (slot)
			{
				case SaveSlot.Slot1:
					return "user://playerData_1.bin";
				case SaveSlot.Slot2:
					return "user://playerData_2.bin";
				case SaveSlot.Slot3:
					return "user://playerData_3.bin";
				default:
					return null;
			}
		}
	}

	public static class LevelScenes
	{
		public const string GameWorld = "res://Scenes/Worlds/GameWorld.tscn";
		public const string TitleScreen = "res://Scenes/UI/TitleScreen/TitleScreenMenu.tscn";
	}
}
