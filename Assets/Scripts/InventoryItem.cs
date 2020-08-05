using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
    public Canvas Canvas;
    public RectTransform RectTransform;
    public CanvasGroup CanvasGroup;
    public Inventory Inventory;
    public int Index;
    public bool Locked = false;
    public int SlotNumber;
    public bool Equipped = false;
    private Vector3 originalPosition;

    public void OnBeginDrag(PointerEventData eventData) {
        CanvasGroup.alpha = .6f;
        CanvasGroup.blocksRaycasts = false;
        originalPosition = RectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData) {
        RectTransform.anchoredPosition += eventData.delta / Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        CanvasGroup.alpha = 1f;
        CanvasGroup.blocksRaycasts = true;
        if(Locked == false || Equipped == true) {
            RectTransform.anchoredPosition = originalPosition;
        }
    }

    public void OnDrop(PointerEventData eventData) {
        if(eventData.pointerDrag.GetComponent<Image>().enabled == true) {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if(Equipped) {
                if(Inventory.Items[inventoryItem.Index].type == Inventory.Items[Index].type) {
                    Inventory.EquipItem(inventoryItem.Index, SlotNumber);
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = RectTransform.anchoredPosition;
                }
            } else if(inventoryItem.Equipped && Inventory.Items[Index].type == Inventory.ItemType.empty) {
                Inventory.RemoveEquippedItem(Index, inventoryItem.SlotNumber);
            }
        }
    }
}
