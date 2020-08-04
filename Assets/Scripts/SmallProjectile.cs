using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SmallProjectile : MonoBehaviour {
    public Rigidbody2D Rigidbody;
    public Vector3 playerPosition;
    private Vector2 aimDirection;
    private const float projectileSpeed = 20f;
    private const float damage = 5f;

    void Start() {
        aimDirection = (playerPosition - transform.position).normalized;
        SetRotation();
        SetVelocity();
    }

    void SetRotation() {
        float angle = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg);
        transform.eulerAngles = new Vector3(0, 0, angle - 180f);
    }

    void SetVelocity() {
        Rigidbody.velocity = aimDirection * projectileSpeed;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("Solid")) {
            Destroy(gameObject);
        } else if(collider.CompareTag("Player")) {
            collider.gameObject.GetComponent<PlayerController>().TakeDamage(5f);
            Destroy(gameObject);
        }
    }
}
