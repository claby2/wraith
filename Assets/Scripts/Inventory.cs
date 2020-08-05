using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    public GameObject Abilities;
    public GameObject CurrentWeapon;
    public GameObject ItemInventory;
    public GameObject ItemSlots;
    public GameObject ItemObject;
    public GameObject ItemSlot;
    public PlayerController PlayerController;
    public ItemInformation ItemInformation;
    public Canvas Canvas;
    public Sprite[] WeaponSprites;
    public Sprite[] AbilitySprites;
    public GameObject[] PlayerInventoryItems;
    public InventoryItem[] PlayerInventoryItemScripts;
    public GameObject[] PlayerInventorySlots;
    public bool Active;
    public enum ItemType {
        empty,
        weapon,
        ability
    };
    public struct Item {
        public ItemType type;
        public int id;
        public Item(ItemType type, int id) {
            this.type = type;
            this.id = id;
        }
    };
    public Item[] Items = new Item[rows * columns];
    private const int rows = 5;
    private const int columns = 5;

    void Awake() {
        InitializePlayerInventorySlots();
        InitializePlayerInventoryItems();
        for(int index = 0; index < (rows * columns); ++index) {
            Items[index].type = ItemType.empty;
        }
        Active = false;
        Toggle(transform, Active);
        RenderItems();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            Active = !Active;
            Toggle(Abilities.transform, !Active);
            Toggle(CurrentWeapon.transform, !Active);
            Toggle(transform, Active);
        }
    }

    // Add an item to the inventory, returns true if successful
    public bool AddItem(ItemType type, int id) {
        for(int i = 0; i < (rows * columns); ++i) {
            if(Items[i].type == ItemType.empty) {
                Item newItem = new Item(type, id);
                Items[i] = newItem;
                RenderItems();
                return true;
            }
        }
        // No empty slot was found
        return false;
    }

    // Swap two items at index1 and index2 in the inventory
    public void SwapItems(int index1, int index2) {
        Item itemBuffer = Items[index1];
        Items[index1] = Items[index2];
        Items[index2] = itemBuffer;
        RenderItems();
    }

    // Equip new ability/weapon and replace inventory slot with swapped ability/weapon
    public void EquipItem(int index, int slotNumber) {
        Item newInventoryItem = new Item();
        newInventoryItem.type = Items[index].type;
        if(Items[index].type == ItemType.weapon) {
            newInventoryItem.id = PlayerController.WeaponId;
            PlayerController.EquipWeapon(Items[index].id);
        } else if(Items[index].type == ItemType.ability) {
            newInventoryItem.id = PlayerController.Abilities[slotNumber].id;
            PlayerController.EquipAbility(slotNumber, Items[index].id);
        }
        Items[index] = newInventoryItem;
        RenderItems();
    }

    // Remove the current equipped item at slotNumber and place it at index
    public void RemoveEquippedItem(int index, int slotNumber) {
        Item newInventoryItem = RemoveEquippedItem(slotNumber);
        Items[index] = newInventoryItem;
        RenderItems();
    }

    // Swap the currently equipped item at slotNumber with the item at index
    public void SwapEquippedItem(int index, int slotNumber) {
        Item newInventoryItem = RemoveEquippedItem(slotNumber);
        EquipItem(index, slotNumber);
        Items[index] = newInventoryItem;
        RenderItems();
    }

    // Remove an item at a given index
    public void RemoveItem(int index) {
        Items[index].type = ItemType.empty;
        RenderItems();
    }

    // Removes the equipped item at slotNumber and returns the item that was removed
    Item RemoveEquippedItem(int slotNumber) {
        Item newInventoryItem = new Item();
        if(slotNumber < 3) {
            // Remove an ability
            newInventoryItem.type = ItemType.ability;
            newInventoryItem.id = PlayerController.Abilities[slotNumber].id;
            PlayerController.EquipAbility(slotNumber, -1);
        } else if(slotNumber == 3) {
            // Remove a weapon
            newInventoryItem.type = ItemType.weapon;
            newInventoryItem.id = PlayerController.WeaponId;
            PlayerController.EquipWeapon(-1);
        }
        return newInventoryItem;
    }

    void InitializePlayerInventorySlots() {
        // Set first three slots as ability and fourth one as weapon
        for(int i = 0; i < 4; ++i) {
            PlayerInventorySlot playerInventorySlot = PlayerInventorySlots[i].GetComponent<PlayerInventorySlot>();
            if(i < 3) {
                playerInventorySlot.Type = ItemType.ability;
            } else {
                playerInventorySlot.Type = ItemType.weapon; 
            }
            playerInventorySlot.Inventory = this;
            playerInventorySlot.SlotNumber = i;
        }
    }

    void InitializePlayerInventoryItems() {
        for(int i = 0; i < 4; ++i) {
            InventoryItem inventoryItem = PlayerInventoryItems[i].GetComponent<InventoryItem>();
            inventoryItem.Canvas = Canvas;
            inventoryItem.Inventory = this;
            inventoryItem.SlotNumber = i;
            inventoryItem.Equipped = true;
            inventoryItem.Type = i == 3 ? ItemType.weapon : ItemType.ability;
            inventoryItem.ItemInformation = ItemInformation;
            inventoryItem.PlayerController = PlayerController;
        }
    }

    void Toggle(Transform transform, bool toggle) {
        foreach(Transform child in transform) {
            child.gameObject.SetActive(toggle);
        }
    }

    void RenderItems() {
        foreach(Transform child in ItemInventory.transform) {
            Destroy(child.gameObject);
        }
        foreach(Transform child in ItemSlots.transform) {
            Destroy(child.gameObject);
        }
        // Render inventory items
        for(int i = 0; i < rows; ++i) {
            for(int j = 0; j < columns; ++j) {
                int index = ((i * columns) + j);
                Vector3 position = new Vector3(
                    (-100 + (j * 50)),
                    (100 - (i * 50)),
                    transform.position.z
                );
                // Instantiate slot
                GameObject itemSlotObject = Instantiate(ItemSlot, ItemSlots.transform.position, Quaternion.identity, ItemSlots.transform);
                itemSlotObject.GetComponent<RectTransform>().anchoredPosition = position;
                InventorySlot inventorySlot = itemSlotObject.GetComponent<InventorySlot>();
                inventorySlot.Index = index;
                inventorySlot.Inventory = this;
                // Instantiate item
                GameObject itemObject = Instantiate(ItemObject, ItemInventory.transform.position, Quaternion.identity, ItemInventory.transform);
                Image itemImage = itemObject.GetComponent<Image>();
                InventoryItem inventoryItem = itemObject.GetComponent<InventoryItem>();
                itemObject.GetComponent<RectTransform>().anchoredPosition = position;
                inventoryItem.Canvas = Canvas;
                inventoryItem.Index = index;
                inventoryItem.Inventory = this;
                inventoryItem.ItemInformation = ItemInformation;
                inventoryItem.PlayerController = PlayerController;
                if(Items[index].type != ItemType.empty && Items[index].id != -1) {
                    itemImage.enabled = true;
                    if(Items[index].type == ItemType.weapon) {
                        itemImage.sprite = WeaponSprites[Items[index].id];
                    } else if(Items[index].type == ItemType.ability) {
                        itemImage.sprite = AbilitySprites[Items[index].id];
                    }
                } else {
                    itemImage.enabled = false;
                }
            }
        }
        // Render player items
        // Iterate through player items
        for(int i = 0; i < 4; ++i) {
            Image itemImage = PlayerInventoryItems[i].GetComponent<Image>();
            PlayerInventoryItemScripts[i].Equipped = true;
            bool enabled = false;
            if(i < 3) {
                // Item is an ability
                int abilityId = PlayerController.Abilities[i].id;
                if(i < PlayerController.Abilities.Length && abilityId != -1) {
                    itemImage.sprite = AbilitySprites[PlayerController.Abilities[i].id];
                    enabled = true;
                }
            } else if(PlayerController.WeaponId != -1) {
                // Item is a weapon
                itemImage.sprite = WeaponSprites[PlayerController.WeaponId];
                enabled = true;
            }
            itemImage.enabled = enabled;
        }
    }
}
