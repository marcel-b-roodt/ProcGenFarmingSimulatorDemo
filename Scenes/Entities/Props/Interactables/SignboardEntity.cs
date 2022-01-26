using Godot;
using System.Collections.Generic;

public class SignboardEntity : StaticBody2D, IDialogueInteractable, ITileEntity
{
	[Export]
	public string[] DialogueText { get; private set; }

	IEntityData IEntity.Data { get { return Data; } }
	public SignboardData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	#region GodotSetup
	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);
	}
	#endregion

	public void Initialise(EntityData entityData)
	{
		Data = (SignboardData)entityData;
	}

	public void Create()
	{

	}

	public void ActivateEntity()
	{ }

	public void DeactivateEntity()
	{ }

	public void Destroy()
	{

	}

	public void InitialiseText(string[] dialogueText)
	{
		//TODO: Put this text inside the Data component so that custom signboards get saved.
		this.DialogueText = dialogueText;
	}


}
