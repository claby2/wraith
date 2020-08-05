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
    public PlayerController playerController;
    public Canvas Canvas;
    public Sprite[] WeaponSprites;
    public Sprite[] AbilitySprites;
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
    private const int rows = 5;
    private const int columns = 5;
    public Item[] Items = new Item[rows * columns];

    void Awake() {
        SetPlayerInventorySlots();
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

    public void SwapItems(int index1, int index2) {
        Item itemBuffer = Items[index1];
        Items[index1] = Items[index2];
        Items[index2] = itemBuffer;
        RenderItems();
    }

    public void EquipItem(int index, int slotNumber) {
        // Equip new ability/weapon and replace inventory slot with swapped ability/weapon
        Item newInventoryItem = new Item();
        newInventoryItem.type = Items[index].type;
        if(Items[index].type == ItemType.weapon) {
            newInventoryItem.id = playerController.WeaponId;
            playerController.EquipWeapon(Items[index].id);
        } else if(Items[index].type == ItemType.ability) {
            newInventoryItem.id = playerController.Abilities[slotNumber].id;
            playerController.EquipAbility(slotNumber, Items[index].id);
        }
        Items[index] = newInventoryItem;
        RenderItems();
    }

    public void RemoveItem(int index) {
        Items[index].type = ItemType.empty;
        RenderItems();
    }

    void SetPlayerInventorySlots() {
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

    void Toggle(Transform transform, bool toggle) {
        foreach(Transform child in transform) {
            child.gameObject.SetActive(toggle);
        }
    }

    void RenderItems() {
        foreach(Transform child in ItemInventory.transform) {
            Destroy(child.gameObject);
        }
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
    }
}
