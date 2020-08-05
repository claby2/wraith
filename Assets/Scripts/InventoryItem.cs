using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public Canvas Canvas;
    public RectTransform RectTransform;
    public CanvasGroup CanvasGroup;
    public Inventory Inventory;
    public Inventory.ItemType Type;
    public ItemInformation ItemInformation;
    public PlayerController PlayerController;
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
                if(Inventory.Items[inventoryItem.Index].type == Type) {
                    Inventory.SwapEquippedItem(inventoryItem.Index, SlotNumber);
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = RectTransform.anchoredPosition;
                }
            } else if(inventoryItem.Equipped && Inventory.Items[Index].type == Inventory.ItemType.empty) {
                Inventory.RemoveEquippedItem(Index, inventoryItem.SlotNumber);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        string name = "";
        string description = "";
        if(Equipped == false) {
            Inventory.Item item = Inventory.Items[Index];
            if(item.type == Inventory.ItemType.ability) {
                PlayerController.Ability ability = PlayerController.AbilityInformation.abilities[item.id];
                name = ability.name;
                description = ability.description;
            } else if(item.type == Inventory.ItemType.weapon) {
                PlayerController.Weapon weapon = PlayerController.WeaponInformation.weapons[item.id];
                name = weapon.name;
                description = weapon.description;
            }
        } else {
            // If equipped, read from equipped items
            if(Type == Inventory.ItemType.ability) {
                PlayerController.Ability ability = PlayerController.Abilities[SlotNumber];
                name = ability.name;
                description = ability.description;
            } else if(Type == Inventory.ItemType.weapon) {
                PlayerController.Weapon weapon = PlayerController.CurrentWeapon;
                name = weapon.name;
                description = weapon.description;
            }
        }
        ItemInformation.SetDisplay(name, description);
    }

    public void OnPointerExit(PointerEventData eventData) {
        // Clear display
        ItemInformation.SetDisplay("", "");
    }
}
