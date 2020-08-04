using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {
    public Camera Camera;

    void Update() {
        if(Cursor.visible) {
            Cursor.visible = false;
        }
        GoToCursor();
    }

    void GoToCursor() {
        Vector3 cursorPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(
            cursorPosition.x,
            cursorPosition.y,
            transform.position.z
        );
    }
}
