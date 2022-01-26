using Godot;
using System.Collections.Generic;

public class ItemPickupEntity : Node2D, IPickupEntity
{
	IEntityData IEntity.Data { get { return Data; } }
	public ItemPickupData Data { get; set; }
	public ulong ID { get { return Data.ID; } }
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	public PlayerInventorySlot ItemSlot { get { return itemSlot; } set { itemSlot = value; } }
	private PlayerInventorySlot itemSlot = new PlayerInventorySlot();
	public bool PickedUp { get; set; }
	public Sprite Sprite
	{
		get { if (sprite == null) { sprite = GetNode<Sprite>("Sprite_Image"); } return sprite; }
	}

	private Sprite sprite;

	public void Initialise(EntityData entityData)
	{

	}

	public void Create()
	{

	}

	public void ActivateEntity()
	{

	}

	public void DeactivateEntity()
	{

	}

	public void Destroy()
	{
		QueueFree();
	}

	public void SetItem(Vector2 position, InventoryItem item, int quantity)
	{
		GlobalPosition = position;
		ItemSlot.SetNewItemData(item, quantity);
		SetSpriteImage(item.Metadata.IconTexture);
	}

	public bool PickUp()
	{
		if (!PickedUp)
		{
			PickedUp = true;
			var iconTexture = itemSlot.Item.Metadata.IconTexture;
			var pickedUpQuantity = Global.Instance.PlayerData.Inventory.AddItem(ref itemSlot);
			//(this as IEntity).PlaySound(SoundType.ItemPickup);
			MessageManager.WriteMessage(GlobalPosition, iconTexture, ToastLength.Short, $"+{pickedUpQuantity}");

			if (ItemSlot.Quantity <= 0)
				Destroy();

			return true;
		}

		return false;
	}

	private void SetSpriteImage(Texture texture)
	{
		this.Sprite.Texture = texture;
		this.Sprite.ScaleImageToTargetSize(48);
	}
}
