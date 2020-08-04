using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentWeaponReload : MonoBehaviour {

    public void SetReload(float currentCooldown, float maximumCooldown) {
        Vector3 scale = transform.localScale;
        scale.y = (Mathf.Max(currentCooldown, 0.0f) / maximumCooldown);
        transform.localScale = scale;
    }
}
