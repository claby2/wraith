using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Ability : MonoBehaviour {
    public Image Image;
    public Sprite SelectedSprite;
    public Sprite UnselectedSprite;

    void Awake() {
        Unselect();
    }

    public void Select() {
        Image.sprite = SelectedSprite;
    }

    public void Unselect() {
        Image.sprite = UnselectedSprite;
    }
}
