using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class InventorySlot : MonoBehaviour, IDropHandler {
    public RectTransform RectTransform;
    public Inventory Inventory;
    public int Index;

    public void OnDrop(PointerEventData eventData) {
        if(eventData.pointerDrag.GetComponent<Image>().enabled == true) {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.Locked = true;
            Inventory.SwapItems(inventoryItem.Index, Index);
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = RectTransform.anchoredPosition;
        }
    }
}
