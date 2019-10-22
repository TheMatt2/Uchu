using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class InventoryHandler : HandlerGroup
    {
        [PacketHandler]
        public void ItemMovementHandler(MoveItemInInventoryMessage message, Player player)
        {
            if (message.DestinationInventoryType == InventoryType.Invalid)
                message.DestinationInventoryType = message.CurrentInventoryType;

            if (message.Item.Inventory.Manager.GameObject != player) return;
            
            message.Item.Slot = (uint) message.NewSlot;
        }
        
        [PacketHandler]
        public void ItemMoveBetweenInventoriesHandler(MoveItemBetweenInventoryTypesMessage message, Player player)
        {
            player.SendChatMessage($"{message.Item?.Lot ?? message.Lot} {message.SourceInventory} -> {message.DestinationInventory}");

            player.GetComponent<InventoryManager>().MoveItemsBetweenInventories(
                message.Item,
                message.Lot,
                message.StackCount,
                message.SourceInventory,
                message.DestinationInventory
            );
        }

        [PacketHandler]
        public void RemoveItemHandler(RemoveItemToInventoryMessage message, Player player)
        {
            if (!message.Confirmed) return;
            
            var inventoryManager = player.GetComponent<InventoryManager>();

            inventoryManager.RemoveItem(message.Item.Lot, message.Delta, message.InventoryType, true);
        }

        [PacketHandler]
        public void EquipItemHandler(EquipItemMessage message, Player player)
        {
            if (message.Item == null) return;
            
            player.GetComponent<InventoryComponent>().EquipItem(message.Item);
        }

        [PacketHandler]
        public void UnEquipItemHandler(UnEquipItemMessage message, Player player)
        {
            var inventoryComponent = player.GetComponent<InventoryComponent>();

            Logger.Information($"UnEquip Item: {message.ItemToUnEquip} | {message.ReplacementItem}");

            inventoryComponent.UnEquipItem(message.ItemToUnEquip);

            if (message.ReplacementItem != null)
                inventoryComponent.EquipItem(message.ReplacementItem);
        }
    }
}