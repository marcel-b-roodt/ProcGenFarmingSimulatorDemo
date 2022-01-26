using Godot;
using Godot.Collections;

public static class ImageStore
{
	public static class UI
	{
		public const string UI_EmptyCell = "res://Resources/Icons/empty_cell.png";
		public const string UI_SelectedCell = "res://Resources/Icons/selected_cell.png";
		public const string UI_TreeNodeArrow_Closed_Normal = "res://Resources/Icons/icon_treenodearrow_closed_normal.png";
		public const string UI_TreeNodeArrow_Closed_Hover = "res://Resources/Icons/icon_treenodearrow_closed_hover.png";
		public const string UI_TreeNodeArrow_Open_Normal = "res://Resources/Icons/icon_treenodearrow_open_normal.png";
		public const string UI_TreeNodeArrow_Open_Hover = "res://Resources/Icons/icon_treenodearrow_open_hover.png";

		#region UI_PCInput
		public const string UI_PC_DPad = "res://Resources/Icons/Input/PC/WASD_Keys_Dark.png";
		public const string UI_PC_DPad_Up = "res://Resources/Icons/Input/PC/W_Key_Dark.png";
		public const string UI_PC_DPad_Down = "res://Resources/Icons/Input/PC/S_Key_Dark.png";
		public const string UI_PC_DPad_Left = "res://Resources/Icons/Input/PC/A_Key_Dark.png";
		public const string UI_PC_DPad_Right = "res://Resources/Icons/Input/PC/D_Key_Dark.png";
		public const string UI_PC_LS = "res://Resources/Icons/Input/PC/WASD_Keys_Dark.png";
		public const string UI_PC_LS_Up = "res://Resources/Icons/Input/PC/W_Key_Dark.png";
		public const string UI_PC_LS_Down = "res://Resources/Icons/Input/PC/S_Key_Dark.png";
		public const string UI_PC_LS_Left = "res://Resources/Icons/Input/PC/A_Key_Dark.png";
		public const string UI_PC_LS_Right = "res://Resources/Icons/Input/PC/D_Key_Dark.png";
		public const string UI_PC_RS = "res://Resources/Icons/Input/PC/Arrow_Keys_Dark.png"; //Not used
		public const string UI_PC_RS_Up = "res://Resources/Icons/Input/PC/Arrow_Up_Key_Dark.png"; //Not used
		public const string UI_PC_RS_Down = "res://Resources/Icons/Input/PC/Arrow_Down_Key_Dark.png"; //Not used
		public const string UI_PC_RS_Left = "res://Resources/Icons/Input/PC/Arrow_Left_Key_Dark.png"; //Not used
		public const string UI_PC_RS_Right = "res://Resources/Icons/Input/PC/Arrow_Right_Key_Dark.png"; //Not used
		public const string UI_PC_A = "res://Resources/Icons/Input/PC/Space_Key_Dark.png";
		public const string UI_PC_B = "res://Resources/Icons/Input/PC/C_Key_Dark.png";
		public const string UI_PC_X = "res://Resources/Icons/Input/PC/F_Key_Dark.png";
		public const string UI_PC_Y = "res://Resources/Icons/Input/PC/R_Key_Dark.png";
		public const string UI_PC_L1 = "res://Resources/Icons/Input/PC/Q_Key_Dark.png";
		public const string UI_PC_L2 = "res://Resources/Icons/Input/PC/1_Key_Dark.png";
		public const string UI_PC_L3 = "res://Resources/Icons/Input/PC/T_Key_Dark.png"; //Not used
		public const string UI_PC_R1 = "res://Resources/Icons/Input/PC/E_Key_Dark.png";
		public const string UI_PC_R2 = "res://Resources/Icons/Input/PC/2_Key_Dark.png";
		public const string UI_PC_R3 = "res://Resources/Icons/Input/PC/G_Key_Dark.png"; //Not used
		public const string UI_PC_Start = "res://Resources/Icons/Input/PC/Enter_Key_Dark.png";
		public const string UI_PC_Select = "res://Resources/Icons/Input/PC/Space_Key_Dark.png";
		#endregion

		#region UI_PlaystationInput
		public const string UI_Playstation_DPad = "res://Resources/Icons/Input/PS3/PS3_Dpad.png";
		public const string UI_Playstation_DPad_Up = "res://Resources/Icons/Input/PS3/PS3_Dpad_Up.png";
		public const string UI_Playstation_DPad_Down = "res://Resources/Icons/Input/PS3/PS3_Dpad_Down.png";
		public const string UI_Playstation_DPad_Left = "res://Resources/Icons/Input/PS3/PS3_Dpad_Left.png";
		public const string UI_Playstation_DPad_Right = "res://Resources/Icons/Input/PS3/PS3_Dpad_Right.png";
		public const string UI_Playstation_LS = "res://Resources/Icons/Input/PS3/PS3_Left_Stick.png";
		public const string UI_Playstation_LS_Up = "res://Resources/Icons/Input/PS3/PS3_Left_Stick.png"; //TODO: Need Directional LS
		public const string UI_Playstation_LS_Down = "res://Resources/Icons/Input/PS3/PS3_Left_Stick.png"; //Need Directional LS
		public const string UI_Playstation_LS_Left = "res://Resources/Icons/Input/PS3/PS3_Left_Stick.png"; //Need Directional LS
		public const string UI_Playstation_LS_Right = "res://Resources/Icons/Input/PS3/PS3_Left_Stick.png"; //Need Directional LS
		public const string UI_Playstation_RS = "res://Resources/Icons/Input/PS3/PS3_Right_Stick.png"; //TODO: Need Directional RS
		public const string UI_Playstation_RS_Up = "res://Resources/Icons/Input/PS3/PS3_Right_Stick.png"; //Need Directional RS
		public const string UI_Playstation_RS_Down = "res://Resources/Icons/Input/PS3/PS3_Right_Stick.png"; //Need Directional RS
		public const string UI_Playstation_RS_Left = "res://Resources/Icons/Input/PS3/PS3_Right_Stick.png"; //Need Directional RS
		public const string UI_Playstation_RS_Right = "res://Resources/Icons/Input/PS3/PS3_Right_Stick.png"; //Need Directional RS
		public const string UI_Playstation_A = "res://Resources/Icons/Input/PS3/PS3_Cross.png";
		public const string UI_Playstation_B = "res://Resources/Icons/Input/PS3/PS3_Circle.png";
		public const string UI_Playstation_X = "res://Resources/Icons/Input/PS3/PS3_Square.png";
		public const string UI_Playstation_Y = "res://Resources/Icons/Input/PS3/PS3_Triangle.png";
		public const string UI_Playstation_L1 = "res://Resources/Icons/Input/PS3/PS3_L1.png";
		public const string UI_Playstation_L2 = "res://Resources/Icons/Input/PS3/PS3_L2.png";
		public const string UI_Playstation_L3 = "res://Resources/Icons/Input/PS3/PS3_Left_Stick_Click.png";
		public const string UI_Playstation_R1 = "res://Resources/Icons/Input/PS3/PS3_R1.png";
		public const string UI_Playstation_R2 = "res://Resources/Icons/Input/PS3/PS3_R2.png";
		public const string UI_Playstation_R3 = "res://Resources/Icons/Input/PS3/PS3_Right_Stick_Click.png";
		public const string UI_Playstation_Start = "res://Resources/Icons/Input/PS3/PS3_Start.png";
		public const string UI_Playstation_Select = "res://Resources/Icons/Input/PS3/PS3_Select.png";
		#endregion

		#region UI_SwitchInput
		public const string UI_Switch_DPad = "res://Resources/Icons/Input/Switch/Switch_Dpad.png";
		public const string UI_Switch_DPad_Up = "res://Resources/Icons/Input/Switch/Switch_Up.png";
		public const string UI_Switch_DPad_Down = "res://Resources/Icons/Input/Switch/Switch_Down.png";
		public const string UI_Switch_DPad_Left = "res://Resources/Icons/Input/Switch/Switch_Left.png";
		public const string UI_Switch_DPad_Right = "res://Resources/Icons/Input/Switch/Switch_Right.png";
		public const string UI_Switch_LS = "res://Resources/Icons/Input/Switch/Switch_Left_Stick.png";
		public const string UI_Switch_LS_Up = "res://Resources/Icons/Input/Switch/Switch_Left_Stick.png";
		public const string UI_Switch_LS_Down = "res://Resources/Icons/Input/Switch/Switch_Left_Stick.png";
		public const string UI_Switch_LS_Left = "res://Resources/Icons/Input/Switch/Switch_Left_Stick.png";
		public const string UI_Switch_LS_Right = "res://Resources/Icons/Input/Switch/Switch_Left_Stick.png";
		public const string UI_Switch_RS = "res://Resources/Icons/Input/Switch/Switch_Right_Stick.png";
		public const string UI_Switch_RS_Up = "res://Resources/Icons/Input/Switch/Switch_Right_Stick.png";
		public const string UI_Switch_RS_Down = "res://Resources/Icons/Input/Switch/Switch_Right_Stick.png";
		public const string UI_Switch_RS_Left = "res://Resources/Icons/Input/Switch/Switch_Right_Stick.png";
		public const string UI_Switch_RS_Right = "res://Resources/Icons/Input/Switch/Switch_Right_Stick.png";
		public const string UI_Switch_A = "res://Resources/Icons/Input/Switch/Switch_B.png";
		public const string UI_Switch_B = "res://Resources/Icons/Input/Switch/Switch_A.png";
		public const string UI_Switch_X = "res://Resources/Icons/Input/Switch/Switch_Y.png";
		public const string UI_Switch_Y = "res://Resources/Icons/Input/Switch/Switch_X.png";
		public const string UI_Switch_L1 = "res://Resources/Icons/Input/Switch/Switch_LB.png";
		public const string UI_Switch_L2 = "res://Resources/Icons/Input/Switch/Switch_LT.png";
		public const string UI_Switch_L3 = "res://Resources/Icons/Input/Switch/Switch_Left_Stick.png";
		public const string UI_Switch_R1 = "res://Resources/Icons/Input/Switch/Switch_RB.png";
		public const string UI_Switch_R2 = "res://Resources/Icons/Input/Switch/Switch_RT.png";
		public const string UI_Switch_R3 = "res://Resources/Icons/Input/Switch/Switch_Right_Stick.png";
		public const string UI_Switch_Start = "res://Resources/Icons/Input/Switch/Switch_Plus.png";
		public const string UI_Switch_Select = "res://Resources/Icons/Input/Switch/Switch_Minus.png";
		#endregion

		#region UI_XboxInput
		public const string UI_Xbox_DPad = @"res://Resources/Icons/Input/Xbox One/XboxOne_Dpad.png";
		public const string UI_Xbox_DPad_Up = @"res://Resources/Icons/Input/Xbox One/XboxOne_Dpad_Up.png";
		public const string UI_Xbox_DPad_Down = @"res://Resources/Icons/Input/Xbox One/XboxOne_Dpad_Down.png";
		public const string UI_Xbox_DPad_Left = @"res://Resources/Icons/Input/Xbox One/XboxOne_Dpad_Left.png";
		public const string UI_Xbox_DPad_Right = @"res://Resources/Icons/Input/Xbox One/XboxOne_Dpad_Right.png";
		public const string UI_Xbox_LS = @"res://Resources/Icons/Input/Xbox One/XboxOne_Left_Stick.png";
		public const string UI_Xbox_LS_Up = @"res://Resources/Icons/Input/Xbox One/XboxOne_Left_Stick.png";
		public const string UI_Xbox_LS_Down = @"res://Resources/Icons/Input/Xbox One/XboxOne_Left_Stick.png";
		public const string UI_Xbox_LS_Left = @"res://Resources/Icons/Input/Xbox One/XboxOne_Left_Stick.png";
		public const string UI_Xbox_LS_Right = @"res://Resources/Icons/Input/Xbox One/XboxOne_Left_Stick.png";
		public const string UI_Xbox_RS = @"res://Resources/Icons/Input/Xbox One/XboxOne_Right_Stick.png";
		public const string UI_Xbox_RS_Up = @"res://Resources/Icons/Input/Xbox One/XboxOne_Right_Stick.png";
		public const string UI_Xbox_RS_Down = @"res://Resources/Icons/Input/Xbox One/XboxOne_Right_Stick.png";
		public const string UI_Xbox_RS_Left = @"res://Resources/Icons/Input/Xbox One/XboxOne_Right_Stick.png";
		public const string UI_Xbox_RS_Right = @"res://Resources/Icons/Input/Xbox One/XboxOne_Right_Stick.png";
		public const string UI_Xbox_A = @"res://Resources/Icons/Input/Xbox One/XboxOne_A.png";
		public const string UI_Xbox_B = @"res://Resources/Icons/Input/Xbox One/XboxOne_B.png";
		public const string UI_Xbox_X = @"res://Resources/Icons/Input/Xbox One/XboxOne_X.png";
		public const string UI_Xbox_Y = @"res://Resources/Icons/Input/Xbox One/XboxOne_Y.png";
		public const string UI_Xbox_L1 = @"res://Resources/Icons/Input/Xbox One/XboxOne_LB.png";
		public const string UI_Xbox_L2 = @"res://Resources/Icons/Input/Xbox One/XboxOne_LT.png";
		public const string UI_Xbox_L3 = @"res://Resources/Icons/Input/Xbox One/XboxOne_Left_Stick_Click.png";
		public const string UI_Xbox_R1 = @"res://Resources/Icons/Input/Xbox One/XboxOne_RB.png";
		public const string UI_Xbox_R2 = @"res://Resources/Icons/Input/Xbox One/XboxOne_RT.png";
		public const string UI_Xbox_R3 = @"res://Resources/Icons/Input/Xbox One/XboxOne_Right_Stick_Click.png";
		public const string UI_Xbox_Start = @"res://Resources/Icons/Input/Xbox One/XboxOne_Menu.png";
		public const string UI_Xbox_Select = @"res://Resources/Icons/Input/Xbox One/XboxOne_Windows.png";
		#endregion
	}

	public static class Items
	{
		public const string Building_Tent = "res://Resources/Icons/icon_tent.png";

		public const string Consumable_Artichoke = "res://Resources/Icons/free_artichoke.png";
		public const string Consumable_Apple = "res://Resources/Icons/mc_apple.png";
		public const string Consumable_Carrot = "res://Resources/Icons/free_carrot.png";
		public const string Consumable_Corn = "res://Resources/Icons/free_corn.png";
		public const string Consumable_Gourd = "res://Resources/Icons/free_gourd.png";
		public const string Consumable_Potato = "res://Resources/Icons/free_potato.png";
		public const string Consumable_Tomato = "res://Resources/Icons/free_tomato.png";
		public const string Consumable_Nametag = "res://Resources/Icons/mc_nametag.png";

		public const string Misc_Grass = "res://Resources/Icons/icon_grass.png";
		public const string Misc_Stone = "res://Resources/Icons/icon_stone.png";
		public const string Misc_WoodenLog = "res://Resources/Icons/icon_wooden_log.png";
		public const string Misc_WoodenStick = "res://Resources/Icons/mc_stick.png";

		public const string Sapling_Pine = "res://Resources/Icons/mc_sapling.png";
		public const string Sapling_Oak = "res://Resources/Icons/mc_sapling.png";
		public const string Sapling_Apple = "res://Resources/Icons/mc_sapling.png";
		public const string Seeds_Artichoke = "res://Resources/Icons/mc_seeds.png";
		public const string Seeds_Apple = "res://Resources/Icons/mc_seeds.png";
		public const string Seeds_Carrot = "res://Resources/Icons/mc_seeds.png";
		public const string Seeds_Corn = "res://Resources/Icons/mc_seeds.png";
		public const string Seeds_Gourd = "res://Resources/Icons/mc_seeds.png";
		public const string Seeds_Potato = "res://Resources/Icons/mc_seeds.png";
		public const string Seeds_Tomato = "res://Resources/Icons/mc_seeds.png";

		public const string Tool_StoneAxe = "res://Resources/Icons/mc_stone_axe.png";
		public const string Tool_WoodenAxe = "res://Resources/Icons/mc_wooden_axe.png";
		public const string Tool_StoneHoe = "res://Resources/Icons/mc_stone_hoe.png";
		public const string Tool_WoodenHoe = "res://Resources/Icons/mc_wooden_hoe.png";
		public const string Tool_StonePickaxe = "res://Resources/Icons/mc_stone_pickaxe.png";
		public const string Tool_WoodenPickaxe = "res://Resources/Icons/mc_wooden_pickaxe.png";
		public const string Tool_StoneSpade = "res://Resources/Icons/mc_stone_shovel.png";
		public const string Tool_WoodenSpade = "res://Resources/Icons/mc_wooden_shovel.png";
	}

	public static class Skills
	{
		public const string Skill_Building = "res://icon.png";
		public const string Skill_Farming = "res://icon.png";
		public const string Skill_Metalworking = "res://icon.png";
		public const string Skill_Mining = "res://icon.png";
		public const string Skill_Toolworking = "res://icon.png";
	}

	public static class Error
	{
		public const string ErrorImage = "res://error.png";
	}

	private static Dictionary<string, Texture> resources = new Dictionary<string, Texture>
	{
		#region UI_Elements
		{ nameof(UI.UI_EmptyCell), ResourceLoader.Load<Texture>(UI.UI_EmptyCell)},
		{ nameof(UI.UI_SelectedCell), ResourceLoader.Load<Texture>(UI.UI_SelectedCell)},
		{ nameof(UI.UI_TreeNodeArrow_Closed_Normal), ResourceLoader.Load<Texture>(UI.UI_TreeNodeArrow_Closed_Normal)},
		{ nameof(UI.UI_TreeNodeArrow_Closed_Hover), ResourceLoader.Load<Texture>(UI.UI_TreeNodeArrow_Closed_Hover)},
		{ nameof(UI.UI_TreeNodeArrow_Open_Normal), ResourceLoader.Load<Texture>(UI.UI_TreeNodeArrow_Open_Normal)},
		{ nameof(UI.UI_TreeNodeArrow_Open_Hover), ResourceLoader.Load<Texture>(UI.UI_TreeNodeArrow_Open_Hover)},
		#endregion

		#region UI_PCInput
		{ nameof(UI.UI_PC_DPad), ResourceLoader.Load<Texture>(UI.UI_PC_DPad)},
		{ nameof(UI.UI_PC_DPad_Up), ResourceLoader.Load<Texture>(UI.UI_PC_DPad_Up)},
		{ nameof(UI.UI_PC_DPad_Down), ResourceLoader.Load<Texture>(UI.UI_PC_DPad_Down)},
		{ nameof(UI.UI_PC_DPad_Left), ResourceLoader.Load<Texture>(UI.UI_PC_DPad_Left)},
		{ nameof(UI.UI_PC_DPad_Right), ResourceLoader.Load<Texture>(UI.UI_PC_DPad_Right)},
		{ nameof(UI.UI_PC_LS), ResourceLoader.Load<Texture>(UI.UI_PC_LS)},
		{ nameof(UI.UI_PC_LS_Up), ResourceLoader.Load<Texture>(UI.UI_PC_LS_Up)},
		{ nameof(UI.UI_PC_LS_Down), ResourceLoader.Load<Texture>(UI.UI_PC_LS_Down)},
		{ nameof(UI.UI_PC_LS_Left), ResourceLoader.Load<Texture>(UI.UI_PC_LS_Left)},
		{ nameof(UI.UI_PC_LS_Right), ResourceLoader.Load<Texture>(UI.UI_PC_LS_Right)},
		{ nameof(UI.UI_PC_RS), ResourceLoader.Load<Texture>(UI.UI_PC_RS)},
		{ nameof(UI.UI_PC_RS_Up), ResourceLoader.Load<Texture>(UI.UI_PC_RS_Up)},
		{ nameof(UI.UI_PC_RS_Down), ResourceLoader.Load<Texture>(UI.UI_PC_RS_Down)},
		{ nameof(UI.UI_PC_RS_Left), ResourceLoader.Load<Texture>(UI.UI_PC_RS_Left)},
		{ nameof(UI.UI_PC_RS_Right), ResourceLoader.Load<Texture>(UI.UI_PC_RS_Right)},
		{ nameof(UI.UI_PC_A), ResourceLoader.Load<Texture>(UI.UI_PC_A)},
		{ nameof(UI.UI_PC_B), ResourceLoader.Load<Texture>(UI.UI_PC_B)},
		{ nameof(UI.UI_PC_X), ResourceLoader.Load<Texture>(UI.UI_PC_X)},
		{ nameof(UI.UI_PC_Y), ResourceLoader.Load<Texture>(UI.UI_PC_Y)},
		{ nameof(UI.UI_PC_L1), ResourceLoader.Load<Texture>(UI.UI_PC_L1)},
		{ nameof(UI.UI_PC_L2), ResourceLoader.Load<Texture>(UI.UI_PC_L2)},
		{ nameof(UI.UI_PC_L3), ResourceLoader.Load<Texture>(UI.UI_PC_L3)},
		{ nameof(UI.UI_PC_R1), ResourceLoader.Load<Texture>(UI.UI_PC_R1)},
		{ nameof(UI.UI_PC_R2), ResourceLoader.Load<Texture>(UI.UI_PC_R2)},
		{ nameof(UI.UI_PC_R3), ResourceLoader.Load<Texture>(UI.UI_PC_R3)},
		{ nameof(UI.UI_PC_Start), ResourceLoader.Load<Texture>(UI.UI_PC_Start)},
		{ nameof(UI.UI_PC_Select), ResourceLoader.Load<Texture>(UI.UI_PC_Select)},
		#endregion

		#region UI_PlaystationInput
		{ nameof(UI.UI_Playstation_DPad), ResourceLoader.Load<Texture>(UI.UI_Playstation_DPad)},
		{ nameof(UI.UI_Playstation_DPad_Up), ResourceLoader.Load<Texture>(UI.UI_Playstation_DPad_Up)},
		{ nameof(UI.UI_Playstation_DPad_Down), ResourceLoader.Load<Texture>(UI.UI_Playstation_DPad_Down)},
		{ nameof(UI.UI_Playstation_DPad_Left), ResourceLoader.Load<Texture>(UI.UI_Playstation_DPad_Left)},
		{ nameof(UI.UI_Playstation_DPad_Right), ResourceLoader.Load<Texture>(UI.UI_Playstation_DPad_Right)},
		{ nameof(UI.UI_Playstation_LS), ResourceLoader.Load<Texture>(UI.UI_Playstation_LS)},
		{ nameof(UI.UI_Playstation_LS_Up), ResourceLoader.Load<Texture>(UI.UI_Playstation_LS_Up)},
		{ nameof(UI.UI_Playstation_LS_Down), ResourceLoader.Load<Texture>(UI.UI_Playstation_LS_Down)},
		{ nameof(UI.UI_Playstation_LS_Left), ResourceLoader.Load<Texture>(UI.UI_Playstation_LS_Left)},
		{ nameof(UI.UI_Playstation_LS_Right), ResourceLoader.Load<Texture>(UI.UI_Playstation_LS_Right)},
		{ nameof(UI.UI_Playstation_RS), ResourceLoader.Load<Texture>(UI.UI_Playstation_RS)},
		{ nameof(UI.UI_Playstation_RS_Up), ResourceLoader.Load<Texture>(UI.UI_Playstation_RS_Up)},
		{ nameof(UI.UI_Playstation_RS_Down), ResourceLoader.Load<Texture>(UI.UI_Playstation_RS_Down)},
		{ nameof(UI.UI_Playstation_RS_Left), ResourceLoader.Load<Texture>(UI.UI_Playstation_RS_Left)},
		{ nameof(UI.UI_Playstation_RS_Right), ResourceLoader.Load<Texture>(UI.UI_Playstation_RS_Right)},
		{ nameof(UI.UI_Playstation_A), ResourceLoader.Load<Texture>(UI.UI_Playstation_A)},
		{ nameof(UI.UI_Playstation_B), ResourceLoader.Load<Texture>(UI.UI_Playstation_B)},
		{ nameof(UI.UI_Playstation_X), ResourceLoader.Load<Texture>(UI.UI_Playstation_X)},
		{ nameof(UI.UI_Playstation_Y), ResourceLoader.Load<Texture>(UI.UI_Playstation_Y)},
		{ nameof(UI.UI_Playstation_L1), ResourceLoader.Load<Texture>(UI.UI_Playstation_L1)},
		{ nameof(UI.UI_Playstation_L2), ResourceLoader.Load<Texture>(UI.UI_Playstation_L2)},
		{ nameof(UI.UI_Playstation_L3), ResourceLoader.Load<Texture>(UI.UI_Playstation_L3)},
		{ nameof(UI.UI_Playstation_R1), ResourceLoader.Load<Texture>(UI.UI_Playstation_R1)},
		{ nameof(UI.UI_Playstation_R2), ResourceLoader.Load<Texture>(UI.UI_Playstation_R2)},
		{ nameof(UI.UI_Playstation_R3), ResourceLoader.Load<Texture>(UI.UI_Playstation_R3)},
		{ nameof(UI.UI_Playstation_Start), ResourceLoader.Load<Texture>(UI.UI_Playstation_Start)},
		{ nameof(UI.UI_Playstation_Select), ResourceLoader.Load<Texture>(UI.UI_Playstation_Select)},
		#endregion

		#region UI_SwitchInput
		{ nameof(UI.UI_Switch_DPad), ResourceLoader.Load<Texture>(UI.UI_Switch_DPad)},
		{ nameof(UI.UI_Switch_DPad_Up), ResourceLoader.Load<Texture>(UI.UI_Switch_DPad_Up)},
		{ nameof(UI.UI_Switch_DPad_Down), ResourceLoader.Load<Texture>(UI.UI_Switch_DPad_Down)},
		{ nameof(UI.UI_Switch_DPad_Left), ResourceLoader.Load<Texture>(UI.UI_Switch_DPad_Left)},
		{ nameof(UI.UI_Switch_DPad_Right), ResourceLoader.Load<Texture>(UI.UI_Switch_DPad_Right)},
		{ nameof(UI.UI_Switch_LS), ResourceLoader.Load<Texture>(UI.UI_Switch_LS)},
		{ nameof(UI.UI_Switch_LS_Up), ResourceLoader.Load<Texture>(UI.UI_Switch_LS_Up)},
		{ nameof(UI.UI_Switch_LS_Down), ResourceLoader.Load<Texture>(UI.UI_Switch_LS_Down)},
		{ nameof(UI.UI_Switch_LS_Left), ResourceLoader.Load<Texture>(UI.UI_Switch_LS_Left)},
		{ nameof(UI.UI_Switch_LS_Right), ResourceLoader.Load<Texture>(UI.UI_Switch_LS_Right)},
		{ nameof(UI.UI_Switch_RS), ResourceLoader.Load<Texture>(UI.UI_Switch_RS)},
		{ nameof(UI.UI_Switch_RS_Up), ResourceLoader.Load<Texture>(UI.UI_Switch_RS_Up)},
		{ nameof(UI.UI_Switch_RS_Down), ResourceLoader.Load<Texture>(UI.UI_Switch_RS_Down)},
		{ nameof(UI.UI_Switch_RS_Left), ResourceLoader.Load<Texture>(UI.UI_Switch_RS_Left)},
		{ nameof(UI.UI_Switch_RS_Right), ResourceLoader.Load<Texture>(UI.UI_Switch_RS_Right)},
		{ nameof(UI.UI_Switch_A), ResourceLoader.Load<Texture>(UI.UI_Switch_A)},
		{ nameof(UI.UI_Switch_B), ResourceLoader.Load<Texture>(UI.UI_Switch_B)},
		{ nameof(UI.UI_Switch_X), ResourceLoader.Load<Texture>(UI.UI_Switch_X)},
		{ nameof(UI.UI_Switch_Y), ResourceLoader.Load<Texture>(UI.UI_Switch_Y)},
		{ nameof(UI.UI_Switch_L1), ResourceLoader.Load<Texture>(UI.UI_Switch_L1)},
		{ nameof(UI.UI_Switch_L2), ResourceLoader.Load<Texture>(UI.UI_Switch_L2)},
		{ nameof(UI.UI_Switch_L3), ResourceLoader.Load<Texture>(UI.UI_Switch_L3)},
		{ nameof(UI.UI_Switch_R1), ResourceLoader.Load<Texture>(UI.UI_Switch_R1)},
		{ nameof(UI.UI_Switch_R2), ResourceLoader.Load<Texture>(UI.UI_Switch_R2)},
		{ nameof(UI.UI_Switch_R3), ResourceLoader.Load<Texture>(UI.UI_Switch_R3)},
		{ nameof(UI.UI_Switch_Start), ResourceLoader.Load<Texture>(UI.UI_Switch_Start)},
		{ nameof(UI.UI_Switch_Select), ResourceLoader.Load<Texture>(UI.UI_Switch_Select)},
		#endregion
	
		#region UI_XboxInput
		{ nameof(UI.UI_Xbox_DPad), ResourceLoader.Load<Texture>(UI.UI_Xbox_DPad)},
		{ nameof(UI.UI_Xbox_DPad_Up), ResourceLoader.Load<Texture>(UI.UI_Xbox_DPad_Up)},
		{ nameof(UI.UI_Xbox_DPad_Down), ResourceLoader.Load<Texture>(UI.UI_Xbox_DPad_Down)},
		{ nameof(UI.UI_Xbox_DPad_Left), ResourceLoader.Load<Texture>(UI.UI_Xbox_DPad_Left)},
		{ nameof(UI.UI_Xbox_DPad_Right), ResourceLoader.Load<Texture>(UI.UI_Xbox_DPad_Right)},
		{ nameof(UI.UI_Xbox_LS), ResourceLoader.Load<Texture>(UI.UI_Xbox_LS)},
		{ nameof(UI.UI_Xbox_LS_Up), ResourceLoader.Load<Texture>(UI.UI_Xbox_LS_Up)},
		{ nameof(UI.UI_Xbox_LS_Down), ResourceLoader.Load<Texture>(UI.UI_Xbox_LS_Down)},
		{ nameof(UI.UI_Xbox_LS_Left), ResourceLoader.Load<Texture>(UI.UI_Xbox_LS_Left)},
		{ nameof(UI.UI_Xbox_LS_Right), ResourceLoader.Load<Texture>(UI.UI_Xbox_LS_Right)},
		{ nameof(UI.UI_Xbox_RS), ResourceLoader.Load<Texture>(UI.UI_Xbox_RS)},
		{ nameof(UI.UI_Xbox_RS_Up), ResourceLoader.Load<Texture>(UI.UI_Xbox_RS_Up)},
		{ nameof(UI.UI_Xbox_RS_Down), ResourceLoader.Load<Texture>(UI.UI_Xbox_RS_Down)},
		{ nameof(UI.UI_Xbox_RS_Left), ResourceLoader.Load<Texture>(UI.UI_Xbox_RS_Left)},
		{ nameof(UI.UI_Xbox_RS_Right), ResourceLoader.Load<Texture>(UI.UI_Xbox_RS_Right)},
		{ nameof(UI.UI_Xbox_A), ResourceLoader.Load<Texture>(UI.UI_Xbox_A)},
		{ nameof(UI.UI_Xbox_B), ResourceLoader.Load<Texture>(UI.UI_Xbox_B)},
		{ nameof(UI.UI_Xbox_X), ResourceLoader.Load<Texture>(UI.UI_Xbox_X)},
		{ nameof(UI.UI_Xbox_Y), ResourceLoader.Load<Texture>(UI.UI_Xbox_Y)},
		{ nameof(UI.UI_Xbox_L1), ResourceLoader.Load<Texture>(UI.UI_Xbox_L1)},
		{ nameof(UI.UI_Xbox_L2), ResourceLoader.Load<Texture>(UI.UI_Xbox_L2)},
		{ nameof(UI.UI_Xbox_L3), ResourceLoader.Load<Texture>(UI.UI_Xbox_L3)},
		{ nameof(UI.UI_Xbox_R1), ResourceLoader.Load<Texture>(UI.UI_Xbox_R1)},
		{ nameof(UI.UI_Xbox_R2), ResourceLoader.Load<Texture>(UI.UI_Xbox_R2)},
		{ nameof(UI.UI_Xbox_R3), ResourceLoader.Load<Texture>(UI.UI_Xbox_R3)},
		{ nameof(UI.UI_Xbox_Start), ResourceLoader.Load<Texture>(UI.UI_Xbox_Start)},
		{ nameof(UI.UI_Xbox_Select), ResourceLoader.Load<Texture>(UI.UI_Xbox_Select)},
		#endregion

		#region Items
		{ nameof(Items.Building_Tent), ResourceLoader.Load<Texture>(Items.Building_Tent)},

		{ nameof(Items.Consumable_Artichoke), ResourceLoader.Load<Texture>(Items.Consumable_Artichoke)},
		{ nameof(Items.Consumable_Apple), ResourceLoader.Load<Texture>(Items.Consumable_Apple)},
		{ nameof(Items.Consumable_Carrot), ResourceLoader.Load<Texture>(Items.Consumable_Carrot)},
		{ nameof(Items.Consumable_Corn), ResourceLoader.Load<Texture>(Items.Consumable_Corn)},
		{ nameof(Items.Consumable_Gourd), ResourceLoader.Load<Texture>(Items.Consumable_Gourd)},
		{ nameof(Items.Consumable_Potato), ResourceLoader.Load<Texture>(Items.Consumable_Potato)},
		{ nameof(Items.Consumable_Tomato), ResourceLoader.Load<Texture>(Items.Consumable_Tomato)},
		{ nameof(Items.Consumable_Nametag), ResourceLoader.Load<Texture>(Items.Consumable_Nametag)},

		{ nameof(Items.Misc_Grass), ResourceLoader.Load<Texture>(Items.Misc_Grass)},
		{ nameof(Items.Misc_Stone), ResourceLoader.Load<Texture>(Items.Misc_Stone)},
		{ nameof(Items.Misc_WoodenLog), ResourceLoader.Load<Texture>(Items.Misc_WoodenLog)},
		{ nameof(Items.Misc_WoodenStick), ResourceLoader.Load<Texture>(Items.Misc_WoodenStick)},

		{ nameof(Items.Sapling_Pine), ResourceLoader.Load<Texture>(Items.Sapling_Pine)},
		{ nameof(Items.Sapling_Oak), ResourceLoader.Load<Texture>(Items.Sapling_Oak)},
		{ nameof(Items.Sapling_Apple), ResourceLoader.Load<Texture>(Items.Sapling_Apple)},

		{ nameof(Items.Seeds_Artichoke), ResourceLoader.Load<Texture>(Items.Seeds_Artichoke)},
		{ nameof(Items.Seeds_Apple), ResourceLoader.Load<Texture>(Items.Seeds_Apple)},
		{ nameof(Items.Seeds_Carrot), ResourceLoader.Load<Texture>(Items.Seeds_Carrot)},
		{ nameof(Items.Seeds_Corn), ResourceLoader.Load<Texture>(Items.Seeds_Corn)},
		{ nameof(Items.Seeds_Gourd), ResourceLoader.Load<Texture>(Items.Seeds_Gourd)},
		{ nameof(Items.Seeds_Potato), ResourceLoader.Load<Texture>(Items.Seeds_Potato)},
		{ nameof(Items.Seeds_Tomato), ResourceLoader.Load<Texture>(Items.Seeds_Tomato)},

		{ nameof(Items.Tool_StoneAxe), ResourceLoader.Load<Texture>(Items.Tool_StoneAxe)},
		{ nameof(Items.Tool_WoodenAxe), ResourceLoader.Load<Texture>(Items.Tool_WoodenAxe)},
		{ nameof(Items.Tool_StoneHoe), ResourceLoader.Load<Texture>(Items.Tool_StoneHoe)},
		{ nameof(Items.Tool_WoodenHoe), ResourceLoader.Load<Texture>(Items.Tool_WoodenHoe)},
		{ nameof(Items.Tool_StonePickaxe), ResourceLoader.Load<Texture>(Items.Tool_StonePickaxe)},
		{ nameof(Items.Tool_WoodenPickaxe), ResourceLoader.Load<Texture>(Items.Tool_WoodenPickaxe)},
		{ nameof(Items.Tool_StoneSpade), ResourceLoader.Load<Texture>(Items.Tool_StoneSpade)},
		{ nameof(Items.Tool_WoodenSpade), ResourceLoader.Load<Texture>(Items.Tool_WoodenSpade)},
		#endregion

		#region Skills
		{ nameof(Skills.Skill_Building), ResourceLoader.Load<Texture>(Skills.Skill_Building)},
		{ nameof(Skills.Skill_Farming), ResourceLoader.Load<Texture>(Skills.Skill_Farming)},
		{ nameof(Skills.Skill_Metalworking), ResourceLoader.Load<Texture>(Skills.Skill_Metalworking)},
		{ nameof(Skills.Skill_Mining), ResourceLoader.Load<Texture>(Skills.Skill_Mining)},
		{ nameof(Skills.Skill_Toolworking), ResourceLoader.Load<Texture>(Skills.Skill_Toolworking)},
		#endregion

		{ nameof(Error.ErrorImage), ResourceLoader.Load<Texture>(Error.ErrorImage)},
	};

	public static Texture Get(string name)
	{
		var hasKey = resources.ContainsKey(name);
		return hasKey ? resources[name] : ErrorImage();
	}

	private static Texture ErrorImage()
	{
		return resources[nameof(Error.ErrorImage)];
	}
}
