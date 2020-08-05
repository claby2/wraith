using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class PlayerInventorySlot : MonoBehaviour, IDropHandler {
    public RectTransform RectTransform;
    public Inventory Inventory;
    public Inventory.ItemType Type;
    public int SlotNumber;

    public void OnDrop(PointerEventData eventData) {
        if(eventData.pointerDrag.GetComponent<Image>().enabled == true) {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if(Inventory.Items[inventoryItem.Index].type == Type) {
                Inventory.EquipItem(inventoryItem.Index, SlotNumber);
            }
        }
    }
}
