using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    public GameObject Enemy;
    public float BaseHealth;
    public float Health;
    public bool DoNotDestroy;

    void Start() {
        Health = BaseHealth;
    }

    void Update() {
        Vector3 scale = transform.localScale;
        scale.x = (Health / BaseHealth);
        transform.localScale = scale;
        if(Health <= 0 && !DoNotDestroy) {
            Destroy(Enemy);
        }
    }
}
