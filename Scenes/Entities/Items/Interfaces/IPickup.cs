public interface IPickupEntity : IEntity
{
	PlayerInventorySlot ItemSlot { get; set; }
	bool PickedUp { get; set; }
	bool PickUp();
}
