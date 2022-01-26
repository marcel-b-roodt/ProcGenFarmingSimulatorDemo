public interface IChoppable : IEntity
{
	int ChopHealth { get; } //Every chop health should drop its HP damage in the resource
	int TakeChopDamage(int chopDamage); //Every chop damage should drop an item;
	void ChopDown();
}

