public interface IAttackable : IEntity
{
	int MaxHealth { get; }
	void TakeDamage(int dmg);
	void Die();
}

