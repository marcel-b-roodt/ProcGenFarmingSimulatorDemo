using Godot;
using System.Collections.Generic;

public interface IEntity
{
	IEntityData Data { get; }
	ulong ID { get; }
	string Name { get; }
	Vector2 Position { get; }
	List<SoundPlayer> SoundPlayers { get; }
	NameTagHolder NameTagHolder { get; set; }

	void Initialise(EntityData entityData); //Setup base data, including Custom Name and restoring properties from Save
	void Create(); //Play startup animation and sounds
	void ActivateEntity(); //Enable events and the like on the object
	void DeactivateEntity(); //Disable events and the like
	void Destroy(); //Play destruction animation and sounds. Do anything else necessary
}

