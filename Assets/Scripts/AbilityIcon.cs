using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityIcon : MonoBehaviour {
    public Image Image;
    public Sprite[] AbilityIconSprites;

    public void SetAbility(int abilityId) {
        if(abilityId == -1) {
            Image.enabled = false;
        } else {
            Image.enabled = true;
            Image.sprite = AbilityIconSprites[abilityId];
        }
    }
}
