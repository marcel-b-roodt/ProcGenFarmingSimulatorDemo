public class PotionConsumableMetadata : ConsumableItemMetadata
{
	public PotionEffect Effect;

	public PotionConsumableMetadata(string iconPath, int stackLimit, float value)
		: base(iconPath, InventoryConsumableType.Potion, stackLimit, value)
	{ }

	public abstract class PotionEffect
	{


		public void ApplyEffect()
		{

		}
	}
}