using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityFrame : MonoBehaviour {
    public Image Image;
    public Sprite Selected;
    public Sprite Active;

    void Awake() {
        Unselect();
    }

    public void Unselect() {
        Image.enabled = false;
    }

    public void Select() {
        Image.enabled = true;
        Image.sprite = Selected;
    }

    public void SetActive() {
        Image.enabled = true;
        Image.sprite = Active;
    }
}
