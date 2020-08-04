using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public GameObject Abilities;
    public GameObject CurrentWeapon;
    public bool Active;

    void Awake() {
        Active = false;
        Toggle(transform, Active);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            Active = !Active;
            Toggle(Abilities.transform, !Active);
            Toggle(CurrentWeapon.transform, !Active);
            Toggle(transform, Active);
        }
    }

    void Toggle(Transform transform, bool toggle) {
        foreach(Transform child in transform) {
            child.gameObject.SetActive(toggle);
        }
    }
}
