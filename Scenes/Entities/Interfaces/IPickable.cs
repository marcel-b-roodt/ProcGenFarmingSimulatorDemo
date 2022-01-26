public interface IPickable : IEntity
{
	int PickHealth { get; } //Every chop health should drop its HP damage in the resource
	int TakePickDamage(int pickDamage); //Every chop damage should drop an item;
	void Break();
}

