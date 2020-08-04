using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityIcon : MonoBehaviour {
    public Image Image;
    public Sprite[] AbilityIconSprites;

    public void SetAbility(int abilityId) {
        Image.sprite = AbilityIconSprites[abilityId];
    }
}
