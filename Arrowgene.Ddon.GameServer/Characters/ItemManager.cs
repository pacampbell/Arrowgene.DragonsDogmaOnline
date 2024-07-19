#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Arrowgene.Ddon.Database;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class ItemManager
    {
        public static readonly List<StorageType> EquipmentStorageTypes = new List<StorageType> { /* StorageType.CharacterEquipment, StorageType.PawnEquipment, */ StorageType.ItemBagEquipment };
        public static readonly List<StorageType> ItemBagStorageTypes = new List<StorageType> { StorageType.ItemBagConsumable, StorageType.ItemBagMaterial, StorageType.ItemBagEquipment, StorageType.ItemBagJob };
        public static readonly List<StorageType> BoxStorageTypes = new List<StorageType> { StorageType.StorageBoxNormal, StorageType.StorageBoxExpansion, StorageType.StorageChest };
        public static readonly List<StorageType> BothStorageTypes = ItemBagStorageTypes.Concat(BoxStorageTypes).ToList();

        private static readonly Dictionary<uint, (WalletType Type, uint Quantity)> ItemIdWalletTypeAndQuantity = new Dictionary<uint, (WalletType Type, uint Amount)>() { 
            {7789, (WalletType.Gold, 1)},
            {7790, (WalletType.Gold, 10)},
            {7791, (WalletType.Gold, 100)},
            {7792, (WalletType.RiftPoints,1)},
            {7793, (WalletType.RiftPoints,10)},
            {7794, (WalletType.RiftPoints,100)},
            {7795, (WalletType.BloodOrbs,1)}, // Doesn't show up 
            {7796, (WalletType.BloodOrbs,10)}, // Doesn't show up
            {7797, (WalletType.BloodOrbs,100)}, // Doesn't show up
            {18742, (WalletType.HighOrbs,1)},
            {18743, (WalletType.HighOrbs,10)},
            {18744, (WalletType.HighOrbs,100)},
            {18828,(WalletType.Gold,7500)},
            {18829,(WalletType.RiftPoints,1250)},
            {18830,(WalletType.BloodOrbs,750)},
            {19508,(WalletType.Gold,1000)},
            {19509,(WalletType.Gold,10000)},
            {19510,(WalletType.RiftPoints,1000)},
            {19511,(WalletType.BloodOrbs,1000)}
            // TODO: Find all items that add wallet points
        };

        public bool IsItemWalletPoint(uint itemId)
        {
            return ItemIdWalletTypeAndQuantity.ContainsKey(itemId);
        }

        public (WalletType walletType, uint itemId) ItemToWalletPoint(uint itemId)
        {
            if (!IsItemWalletPoint(itemId))
            {
                return (WalletType.None, 0);
            }
            return ItemIdWalletTypeAndQuantity[itemId];
        }

        // [[item]]
        // id = 16822 (Adds 100 XP)
        // old = '経験値結晶'
        // new = 'Experience Crystal'
        // [[item]]
        // id = 16831 (Adds 10000 XP)
        // old = '経験値結晶'
        // new = 'Experience Crystal'
        // [[item]]
        // id = 18831 (Adds 63000 XP)
        // old = '経験値結晶'
        // new = 'Experience Crystal'

        // [[item]]
        // id = 18832 (Adds 18 PP)
        // old = 'プレイポイント'
        // new = 'Play Point'
        // [[item]]
        // id = 25651 (Adds 1 PP)
        // old = 'プレイポイント'
        // new = 'Play Point'
        // [[item]]
        // id = 25652 (Adds 10 PP)
        // old = 'プレイポイント'
        // new = 'Play Point'
        // [[item]]
        // id = 25653 (Adds 100 PP)
        // old = 'プレイポイント'
        // new = 'Play Point'

        public void GatherItem(DdonServer<GameClient> server, Character character, S2CItemUpdateCharacterItemNtc ntc, InstancedGatheringItem gatheringItem, uint pickedGatherItems)
        {
            if(ItemIdWalletTypeAndQuantity.ContainsKey(gatheringItem.ItemId)) {
                var walletTypeAndQuantity = ItemIdWalletTypeAndQuantity[gatheringItem.ItemId];
                uint totalQuantityToAdd = walletTypeAndQuantity.Quantity * gatheringItem.ItemNum;
                
                CDataWalletPoint characterWalletPoint = character.WalletPointList.Where(wp => wp.Type == walletTypeAndQuantity.Type).First();
                characterWalletPoint.Value += totalQuantityToAdd; // TODO: Cap to maximum for that wallet
                server.Database.UpdateWalletPoint(character.CharacterId, characterWalletPoint);

                CDataUpdateWalletPoint walletUpdate = new CDataUpdateWalletPoint();
                walletUpdate.Type = walletTypeAndQuantity.Type;
                walletUpdate.AddPoint = (int) totalQuantityToAdd;
                walletUpdate.Value = characterWalletPoint.Value;
                ntc.UpdateWalletList.Add(walletUpdate);
                
                gatheringItem.ItemNum -= pickedGatherItems;
            } else {
                List<CDataItemUpdateResult> results = AddItem(server, character, true, gatheringItem.ItemId, pickedGatherItems);
                ntc.UpdateItemList.AddRange(results);
                gatheringItem.ItemNum -= (uint) results.Select(result => result.UpdateItemNum).Sum();
            }
        }

        public List<CDataItemUpdateResult> ConsumeItemByUIdFromMultipleStorages(DdonServer<GameClient> server, Character character, List<StorageType> fromStorageTypes, string itemUId, uint consumeNum)
        {
            int remainingItems = (int) consumeNum;
            List<CDataItemUpdateResult> results = new List<CDataItemUpdateResult>();
            foreach (StorageType storageType in fromStorageTypes)
            {
                CDataItemUpdateResult? result = ConsumeItemByUId(server, character, storageType, itemUId, (uint) remainingItems);
                if (result != null)
                {
                    results.Add(result);
                    remainingItems += result.UpdateItemNum;
                    if (remainingItems == 0)
                    {
                        return results;
                    }
                }
            }

            // TODO: Rollback transaction
            throw new NotEnoughItemsException(itemUId, consumeNum, remainingItems);
        }

        public CDataItemUpdateResult? ConsumeItemByUId(DdonServer<GameClient> server, Character character, StorageType fromStorageType, string itemUId, uint consumeNum)
        {
            var foundItem = character.Storage.GetStorage(fromStorageType).FindItemByUId(itemUId);
            if(foundItem == null)
            {
                return null;
            } else {
                (ushort slotNo, Item item, uint itemNum) = foundItem;
                return ConsumeItem(server, character, fromStorageType, slotNo, item, itemNum, consumeNum);
            }
        }

        public CDataItemUpdateResult? ConsumeItemByUIdFromItemBag(DdonServer<GameClient> server, Character character, string itemUId, uint consumeNum)
        {
            List<StorageType> itemBagStorage = new List<StorageType>() { StorageType.ItemBagConsumable, StorageType.ItemBagEquipment, StorageType.ItemBagJob, StorageType.ItemBagMaterial, StorageType.KeyItems };
            List<CDataItemUpdateResult> results = ConsumeItemByUIdFromMultipleStorages(server, character, itemBagStorage, itemUId, consumeNum);
            return results.Count > 0 ? results[0] : null;
        }

        public CDataItemUpdateResult? ConsumeItemInSlot(DdonServer<GameClient> server, Character character, StorageType fromStorageType, ushort slotNo, uint consumeNum)
        {
            var foundItem = character.Storage.GetStorageItem(fromStorageType, slotNo);
            if(foundItem == null)
            {
                return null;
            } else {
                (Item item, uint itemNum) = foundItem;
                return ConsumeItem(server, character, fromStorageType, slotNo, item, itemNum, consumeNum);
            }
        }

        private CDataItemUpdateResult ConsumeItem(DdonServer<GameClient> server, Character character, StorageType fromStorageType, ushort slotNo, Item item, uint itemNum, uint consuneNum)
        {
            uint finalItemNum = (uint) Math.Max(0, (int)itemNum - (int)consuneNum);
            int finalConsumeNum = (int)itemNum - (int)finalItemNum;

            CDataItemUpdateResult ntcData = new CDataItemUpdateResult();
            ntcData.ItemList.ItemUId = item.UId;
            ntcData.ItemList.ItemId = item.ItemId;
            ntcData.ItemList.ItemNum = finalItemNum;
            ntcData.ItemList.Unk3 = item.Unk3;
            ntcData.ItemList.StorageType = fromStorageType;
            ntcData.ItemList.SlotNo = slotNo;
            ntcData.ItemList.Color = item.Color;
            ntcData.ItemList.PlusValue = item.PlusValue;
            ntcData.ItemList.Bind = false;
            ntcData.ItemList.EquipPoint = 0;
            ntcData.ItemList.EquipCharacterID = 0;
            ntcData.ItemList.EquipPawnID = 0;
            ntcData.ItemList.WeaponCrestDataList = item.WeaponCrestDataList;
            ntcData.ItemList.ArmorCrestDataList = item.ArmorCrestDataList;
            ntcData.ItemList.EquipElementParamList = item.EquipElementParamList;
            ntcData.UpdateItemNum = -finalConsumeNum;

            if(finalItemNum == 0)
            {
                // Delete item when ItemNum reaches 0 to free up the slot
                character.Storage.SetStorageItem(null, 0, fromStorageType, slotNo);
                server.Database.DeleteStorageItem(character.CharacterId, fromStorageType, slotNo);
            }
            else
            {
                character.Storage.SetStorageItem(item, finalItemNum, fromStorageType, slotNo);
                server.Database.UpdateStorageItem(character.CharacterId, fromStorageType, slotNo, item, finalItemNum);
            }

            return ntcData;
        }

        public List<CDataItemUpdateResult> AddItem(DdonServer<GameClient> server, Character character, bool itemBag, uint itemId, uint num)
        {
            ClientItemInfo clientItemInfo = ClientItemInfo.GetInfoForItemId(server.AssetRepository.ClientItemInfos, itemId);
            if(itemBag)
            {
                // Limit stacks when adding to the item bag.
                return DoAddItem(server.Database, character, clientItemInfo.StorageType, itemId, num, clientItemInfo.StackLimit);
            }
            else
            {
                // TODO: Support adding to the extension boxes if the storage box is full and the GG course allows it
                if(clientItemInfo.StorageType == StorageType.ItemBagEquipment)
                {
                    // Equipment is a special case. It can't be stacked, even on the storage box. So we limit in there too
                    return DoAddItem(server.Database, character, StorageType.StorageBoxNormal, itemId, num, clientItemInfo.StackLimit);
                }
                else
                {
                    // Move to storage box without stack limit if it's not equipment
                    return DoAddItem(server.Database, character, StorageType.StorageBoxNormal, itemId, num);
                }
            }
        }

        private List<CDataItemUpdateResult> DoAddItem(IDatabase database, Character character, StorageType destinationStorageType, uint itemId, uint num, uint stackLimit = UInt32.MaxValue)
        {
            // Add to existing stacks or make new stacks until there are no more items to add
            // The stack limit is specified by the stackLimit arg
            List<CDataItemUpdateResult> results = new List<CDataItemUpdateResult>();
            uint itemsToAdd = num;
            while(itemsToAdd > 0)
            {
                var itemAndNumWithSlot = character.Storage.GetStorage(destinationStorageType).Items
                    .Select((itemAndCount, index) => new {item = itemAndCount, slot = (ushort) (index + 1)})
                    .Where(itemAndNumWithSlot => (
                        itemAndNumWithSlot.item?.Item1.ItemId == itemId
                        && itemAndNumWithSlot.item?.Item2 < stackLimit
                    ))
                    .FirstOrDefault();

                Item? item = itemAndNumWithSlot?.item?.Item1;
                ushort slot = itemAndNumWithSlot?.slot ?? 0;
                uint oldItemNum = itemAndNumWithSlot?.item?.Item2 ?? 0;
                uint newItemNum = Math.Min(stackLimit, oldItemNum + itemsToAdd);
                uint addedItems = newItemNum - oldItemNum;
                itemsToAdd -= addedItems;
                
                if (item == null)
                {
                    item = new Item() {
                        ItemId = itemId,
                        Unk3 = 0,
                        Color = 0,
                        PlusValue = 0,
                        WeaponCrestDataList = new List<CDataWeaponCrestData>(),
                        ArmorCrestDataList = new List<CDataArmorCrestData>(),
                        EquipElementParamList = new List<CDataEquipElementParam>()
                    };
                    slot = character.Storage.AddStorageItem(item, newItemNum, destinationStorageType);
                    database.InsertStorageItem(character.CharacterId, destinationStorageType, slot, item, newItemNum);
                }
                else
                {
                    character.Storage.SetStorageItem(item, newItemNum, destinationStorageType, slot);
                    database.UpdateStorageItem(character.CharacterId, destinationStorageType, slot, item, newItemNum);
                }

                CDataItemUpdateResult result = new CDataItemUpdateResult();
                result.ItemList.ItemUId = item.UId;
                result.ItemList.ItemId = item.ItemId;
                result.ItemList.ItemNum = newItemNum;
                result.ItemList.Unk3 = item.Unk3;
                result.ItemList.StorageType = destinationStorageType;
                result.ItemList.SlotNo = slot;
                result.ItemList.Color = item.Color; // ?
                result.ItemList.PlusValue = item.PlusValue; // ?
                result.ItemList.Bind = false;
                result.ItemList.EquipPoint = 0;
                result.ItemList.EquipCharacterID = 0;
                result.ItemList.EquipPawnID = 0;
                result.ItemList.WeaponCrestDataList = item.WeaponCrestDataList;
                result.ItemList.ArmorCrestDataList = item.ArmorCrestDataList;
                result.ItemList.EquipElementParamList = item.EquipElementParamList;
                result.UpdateItemNum = (int) addedItems;
                results.Add(result);
            }

            return results;
        }

        private bool RemoveStorageItem(IDatabase database, Character character, Item item, StorageType storage, ushort slotNo)
        {
            character.Storage.SetStorageItem(null, 0, storage, slotNo);
            database.DeleteStorageItem(character.CharacterId, storage, slotNo);
            return true;
        }

        private bool UpdateStorageItem(IDatabase database, Character character, Item item, StorageType storage, ushort slotNo, uint amount)
        {
            character.Storage.SetStorageItem(item, amount, storage, slotNo);
            database.UpdateStorageItem(character.CharacterId, storage, slotNo, item, amount);
            return true;
        }

        private uint CalculateAmountToMove(uint itemsToMove, uint oldDstItemNum, uint stackLimit)
        {
            uint movedItemNum = 0;

            if ((stackLimit - oldDstItemNum) < (oldDstItemNum + itemsToMove))
            {
                movedItemNum = stackLimit - oldDstItemNum;
            }
            else
            {
                movedItemNum = itemsToMove;
            }

            return movedItemNum;
        }

        public List<CDataItemUpdateResult> MoveItem(DdonServer<GameClient> server, Character character, StorageType fromStorage, string itemUId, uint num, StorageType toStorage, ushort toSlotNo)
        {
            List<CDataItemUpdateResult> results = new List<CDataItemUpdateResult>();

            uint itemId = LookupItemByUId(server, itemUId);

            // Figure out stack limit in destination storage
            uint stackLimit = uint.MaxValue;
            ClientItemInfo clientItemInfo = ClientItemInfo.GetInfoForItemId(server.AssetRepository.ClientItemInfos, itemId);
            if(clientItemInfo.StorageType == StorageType.ItemBagEquipment || ItemBagStorageTypes.Contains(toStorage))
            {
                // Limit items to the item bag stack limit when moving to the item bag or when moving equipment
                stackLimit = clientItemInfo.StackLimit;
            }

            // Obtain source item information
            //           item1, item2, slot
            // tuple = ((item, count), slot)
            var tuple = character.Storage.GetStorage(fromStorage).Items
                .Select((item, index) => new { item, slot = (ushort)(index + 1) })
                .Where(tuple => itemUId == tuple.item?.Item1.UId && tuple.item?.Item2 >= num)
                .First();
            Item itemToMove = tuple.item!.Item1;
            ushort fromSlotNo = tuple.slot;
            uint oldSrcItemNum = tuple.item.Item2;
            uint oldDstItemNum = 0;

            // Remove items from source storage
            uint newSrcItemNum = oldSrcItemNum - num;
            if(newSrcItemNum == 0)
            {
                RemoveStorageItem(server.Database, character, itemToMove, fromStorage, fromSlotNo);
            }
            else
            {
                UpdateStorageItem(server.Database, character, itemToMove, fromStorage, fromSlotNo, newSrcItemNum);
            }

            // Update the source position data
            CDataItemUpdateResult srcUpdateItem = new CDataItemUpdateResult();
            srcUpdateItem.ItemList.ItemUId = itemToMove.UId;
            srcUpdateItem.ItemList.ItemId = itemToMove.ItemId;
            srcUpdateItem.ItemList.ItemNum = newSrcItemNum;
            srcUpdateItem.ItemList.Unk3 = itemToMove.Unk3;
            srcUpdateItem.ItemList.StorageType = fromStorage;
            srcUpdateItem.ItemList.SlotNo = fromSlotNo;
            srcUpdateItem.ItemList.Color = itemToMove.Color; // ?
            srcUpdateItem.ItemList.PlusValue = itemToMove.PlusValue; // ?
            srcUpdateItem.ItemList.Bind = false;
            srcUpdateItem.ItemList.EquipPoint = 0;
            srcUpdateItem.ItemList.EquipCharacterID = 0;
            srcUpdateItem.ItemList.EquipPawnID = 0;
            srcUpdateItem.ItemList.WeaponCrestDataList = itemToMove.WeaponCrestDataList;
            srcUpdateItem.ItemList.ArmorCrestDataList = itemToMove.ArmorCrestDataList;
            srcUpdateItem.ItemList.EquipElementParamList = itemToMove.EquipElementParamList;
            srcUpdateItem.UpdateItemNum = (int) -num;
            results.Add(srcUpdateItem);

            // Calculate the destination data
            uint itemsToMove = num;
            Item item = (newSrcItemNum == 0) ? itemToMove : new Item(itemToMove);
            for (int i = 0; itemsToMove > 0; i++)
            {
                bool isCreate = false;
                ushort dstSlotNo;
                if (toSlotNo != 0)
                {
                    Tuple<Item, uint>? itemInDstSlot = character.Storage.GetStorageItem(toStorage, toSlotNo);
                    if (itemInDstSlot != null)
                    {
                        if (itemInDstSlot.Item1.ItemId != clientItemInfo.ItemId || stackLimit == 1)
                        {
                            if (stackLimit == 1)
                            {
                                isCreate = true;
                            }

                            // If there's an item in it, and it's not of the same type or the stack size is 1 (equipment) swap items.
                            // Move the item in the destination slot to the source slot
                            results.AddRange(MoveItem(server, character, toStorage, itemInDstSlot.Item1.UId, itemInDstSlot.Item2, fromStorage, fromSlotNo));
                        }
                        else
                        {

                            // If there's an item in it, and it's of the same type, add both counts
                            // TODO: Verify an infinite loop can't happen if it tries to add over the stack limit
                            // in a slot that already has items
                            item = itemInDstSlot.Item1;
                            oldDstItemNum = itemInDstSlot.Item2;
                        }
                    }
                    else
                    {
                        oldDstItemNum = 0;
                        dstSlotNo = character.Storage.AddStorageItem(item, oldDstItemNum, toStorage);
                        isCreate = true;
                    }
                    dstSlotNo = toSlotNo;
                }
                else
                {
                    // Search for a stack to merge with
                    var itemInDstStorage = character.Storage.GetStorage(toStorage).Items
                        .Select((item, index) => new { item, index })
                        .Where(tuple => clientItemInfo.ItemId == tuple.item?.Item1.ItemId && tuple.item?.Item2 < stackLimit)
                        .FirstOrDefault();
                    
                    if (itemInDstStorage != null)
                    {
                        // If there is, use that item's stack slot
                        item = itemInDstStorage.item!.Item1;
                        oldDstItemNum = itemInDstStorage.item!.Item2;
                        dstSlotNo = (ushort)(itemInDstStorage.index + 1);
                    }
                    else
                    {
                        oldDstItemNum = 0;
                        dstSlotNo = character.Storage.AddStorageItem(item, oldDstItemNum, toStorage);
                        isCreate = true;
                    }
                }

                uint movedItemNum = CalculateAmountToMove(itemsToMove, oldDstItemNum, stackLimit);
                uint newDstItemNum = movedItemNum + oldDstItemNum;
                character.Storage.SetStorageItem(item, newDstItemNum, toStorage, dstSlotNo);

                if (isCreate)
                {
                    server.Database.InsertStorageItem(character.CharacterId, toStorage, dstSlotNo, item, newDstItemNum);
                }
                else
                {
                    server.Database.UpdateStorageItem(character.CharacterId, toStorage, dstSlotNo, item, newDstItemNum);
                }

                CDataItemUpdateResult dstUpdateItem = new CDataItemUpdateResult();
                dstUpdateItem.ItemList.ItemUId = item.UId;
                dstUpdateItem.ItemList.ItemId = item.ItemId;
                dstUpdateItem.ItemList.ItemNum = newDstItemNum;
                dstUpdateItem.ItemList.Unk3 = item.Unk3;
                dstUpdateItem.ItemList.StorageType = toStorage;
                dstUpdateItem.ItemList.SlotNo = dstSlotNo;
                dstUpdateItem.ItemList.Color = item.Color; // ?
                dstUpdateItem.ItemList.PlusValue = item.PlusValue; // ?
                dstUpdateItem.ItemList.Bind = false;
                dstUpdateItem.ItemList.EquipPoint = 0;
                dstUpdateItem.ItemList.EquipCharacterID = 0;
                dstUpdateItem.ItemList.EquipPawnID = 0;
                dstUpdateItem.ItemList.WeaponCrestDataList = item.WeaponCrestDataList;
                dstUpdateItem.ItemList.ArmorCrestDataList = item.ArmorCrestDataList;
                dstUpdateItem.ItemList.EquipElementParamList = item.EquipElementParamList;
                dstUpdateItem.UpdateItemNum = (int)movedItemNum;
                results.Add(dstUpdateItem);

                itemsToMove -= movedItemNum;

                // Handle splitting stacks (generate a new UID)
                if (itemsToMove > 0)
                {
                    item = new Item(item);
                }

                // Set toSlotNo=0 so we look for a new spot in the next iteration
                toSlotNo = 0;
            }

            return results;
        }

        public uint LookupItemByUId(DdonServer<GameClient> server, string itemUId)
        {
            var item = server.Database.SelectStorageItemByUId(itemUId);
            if (item == null)
            {
                throw new ItemDoesntExistException(itemUId);
            }

            return item.ItemId;
        }
    }

    [Serializable]
    internal class ItemDoesntExistException : Exception
    {
        private string itemUID;

        public ItemDoesntExistException(string itemUID) : base ($"An item with the UID ${itemUID} is missing in the database")
        {
            this.itemUID = itemUID;
        }
    }

    [Serializable]
    internal class NotEnoughItemsException : Exception
    {
        private string itemUId;
        private uint consumeNum;
        private int remainingItems;

        public NotEnoughItemsException(string itemUId, uint consumeNum, int remainingItems) : base($"Required {consumeNum} items of UID {itemUId}, missing {remainingItems} items")
        {
            this.itemUId = itemUId;
            this.consumeNum = consumeNum;
            this.remainingItems = remainingItems;
        }
    }
}
