using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject Player;

    void LateUpdate() {
        if(transform.position != Player.transform.position) {
            Follow();
        }
    }

    void Follow() {
        Vector3 targetPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
        transform.position = targetPosition;
    }
}
