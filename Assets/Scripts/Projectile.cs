using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour {
    public Rigidbody2D Rigidbody;
    public SpriteRenderer SpriteRenderer;
    public Vector3 CrosshairPosition;
    public Sprite[] ProjectileSprites;
    public float Speed;
    public float Damage;
    public int SpriteId;
    private Vector2 aimDirection;
    private float angleOffset;

    void Start() {
        aimDirection = (CrosshairPosition - transform.position).normalized;
        SpriteRenderer.sprite = ProjectileSprites[SpriteId];
        switch(SpriteId) {
            case 0:
            case 2:
                angleOffset = 180f;
                break;
            case 1:
                angleOffset = 225f;
                break;
            default:
                angleOffset = 0f;
                break;
        }
        SetRotation();
        SetVelocity();
    }

    void SetRotation() {
        float angle = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg);
        transform.eulerAngles = new Vector3(0, 0, angle - angleOffset);
    }

    void SetVelocity() {
        Rigidbody.velocity = aimDirection * Speed;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("Solid")) {
            Destroy(gameObject);
        } else if(collider.CompareTag("Wisp")) {
            collider.gameObject.GetComponent<Wisp>().TakeDamage(Damage);
            Destroy(gameObject);
        } else if(collider.CompareTag("Necromancer")) {
            collider.gameObject.GetComponent<Necromancer>().TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
