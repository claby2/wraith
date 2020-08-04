using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour {

    public void SetHealth(float currentHealth, float maximumHealth) {
        Vector3 scale = transform.localScale;
        scale.x = (currentHealth / maximumHealth);
        transform.localScale = scale;
    }
}
