using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public Canvas Canvas;
    public RectTransform RectTransform;
    public CanvasGroup CanvasGroup;
    public int Index;
    public bool Locked = false;
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
        if(Locked == false) {
            RectTransform.anchoredPosition = originalPosition;
        }
    }
}
