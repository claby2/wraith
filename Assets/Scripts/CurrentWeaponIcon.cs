using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CurrentWeaponIcon : MonoBehaviour {
    public Image Image;
    public Sprite[] WeaponSprites;

    public void SetWeapon(int id) {
        if(id != -1) {
            Image.enabled = true;
            Image.sprite = WeaponSprites[id];
        } else {
            Image.enabled = false;
        }
    }
}
