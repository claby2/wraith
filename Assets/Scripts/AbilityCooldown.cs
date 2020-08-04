using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityCooldown : MonoBehaviour {
    public Image Image;

    public void SetCooldown(float currentCooldown, float cooldown) {
        if(currentCooldown <= 0) {
            Image.enabled = false;
        } else {
            Image.enabled = true;
            Vector3 scale = transform.localScale;
            scale.x = (currentCooldown / cooldown);
            transform.localScale = scale;
        }
    }
}
