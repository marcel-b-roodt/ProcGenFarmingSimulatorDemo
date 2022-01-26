using Godot;
using static Helpers;

public class UIInput : Control
{
	[Export]
	public ButtonType InputButton;
	[Export]
	public string Text = "";

	private TextureRect buttonImage;
	private Label buttonText;

	public override void _Ready()
	{
		buttonImage = GetNode<TextureRect>("ButtonTypeImage");
		buttonText = GetNode<Label>("Text");

		SelectButtonImage();
		buttonText.Text = Text;

		EventManager.ListenEvent(nameof(GlobalEventCodes.Global_PlayerInputUpdated), GD.FuncRef(this, nameof(SelectButtonImage)));
	}

	private void SelectButtonImage()
	{
		switch (InputButton)
		{
			case ButtonType.Dpad:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_DPad));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_DPad));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_DPad));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_DPad));
							return;
					}

					break;
				}
			case ButtonType.DPad_Up:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_DPad_Up));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_DPad_Up));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_DPad_Up));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_DPad_Up));
							return;
					}

					break;
				}
			case ButtonType.DPad_Down:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_DPad_Down));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_DPad_Down));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_DPad_Down));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_DPad_Down));
							return;
					}

					break;
				}
			case ButtonType.DPad_Left:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_DPad_Left));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_DPad_Left));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_DPad_Left));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_DPad_Left));
							return;
					}

					break;
				}
			case ButtonType.DPad_Right:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_DPad_Right));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_DPad_Right));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_DPad_Right));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_DPad_Right));
							return;
					}

					break;
				}
			case ButtonType.LS:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_LS));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_LS));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_LS));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_LS));
							return;
					}

					break;
				}
			case ButtonType.LS_Up:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_LS_Up));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_LS_Up));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_LS_Up));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_LS_Up));
							return;
					}

					break;
				}
			case ButtonType.LS_Down:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_LS_Down));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_LS_Down));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_LS_Down));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_LS_Down));
							return;
					}

					break;
				}
			case ButtonType.LS_Left:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_LS_Left));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_LS_Left));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_LS_Left));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_LS_Left));
							return;
					}

					break;
				}
			case ButtonType.LS_Right:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_LS_Right));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_LS_Right));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_LS_Right));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_LS_Right));
							return;
					}

					break;
				}
			case ButtonType.RS:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_RS));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_RS));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_RS));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_RS));
							return;
					}

					break;
				}
			case ButtonType.RS_Up:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_RS_Up));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_RS_Up));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_RS_Up));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_RS_Up));
							return;
					}

					break;
				}
			case ButtonType.RS_Down:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_RS_Down));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_RS_Down));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_RS_Down));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_RS_Down));
							return;
					}

					break;
				}
			case ButtonType.RS_Left:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_RS_Left));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_RS_Left));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_RS_Left));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_RS_Left));
							return;
					}

					break;
				}
			case ButtonType.RS_Right:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_RS_Right));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_RS_Right));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_RS_Right));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_RS_Right));
							return;
					}

					break;
				}
			case ButtonType.A:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_A));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_A));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_A));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_A));
							return;
					}

					break;
				}
			case ButtonType.B:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_B));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_B));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_B));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_B));
							return;
					}

					break;
				}
			case ButtonType.X:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_X));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_X));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_X));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_X));
							return;
					}

					break;
				}
			case ButtonType.Y:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_Y));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_Y));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_Y));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_Y));
							return;
					}

					break;
				}
			case ButtonType.L1:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_L1));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_L1));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_L1));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_L1));
							return;
					}

					break;
				}
			case ButtonType.L2:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_L2));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_L2));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_L2));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_L2));
							return;
					}

					break;
				}
			case ButtonType.L3:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_L3));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_L3));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_L3));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_L3));
							return;
					}

					break;
				}
			case ButtonType.R1:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_R1));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_R1));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_R1));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_R1));
							return;
					}

					break;
				}
			case ButtonType.R2:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_R2));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_R2));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_R2));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_R2));
							return;
					}

					break;
				}
			case ButtonType.R3:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_R3));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_R3));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_R3));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_R3));
							return;
					}

					break;
				}
			case ButtonType.Start:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_Start));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_Start));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_Start));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_Start));
							return;
					}

					break;
				}
			case ButtonType.Select:
				{
					switch (Global.Instance.ActiveInputType)
					{
						case GamepadType.PC:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_PC_Select));
							return;
						case GamepadType.Playstation:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Playstation_Select));
							return;
						case GamepadType.Switch:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Switch_Select));
							return;
						case GamepadType.Xbox:
							buttonImage.Texture = ImageStore.Get(nameof(ImageStore.UI.UI_Xbox_Select));
							return;
					}

					break;
				}
		}
	}

	public enum ButtonType
	{
		Dpad,
		DPad_Up,
		DPad_Down,
		DPad_Left,
		DPad_Right,
		LS,
		LS_Up,
		LS_Down,
		LS_Left,
		LS_Right,
		RS,
		RS_Up,
		RS_Down,
		RS_Left,
		RS_Right,
		A,
		B,
		X,
		Y,
		L1,
		L2,
		L3,
		R1,
		R2,
		R3,
		Start,
		Select
	}
}

public enum GamepadType
{
	PC,
	Switch,
	Playstation,
	Xbox,
}
