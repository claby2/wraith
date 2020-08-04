using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour {
    public GameObject Player;
    public GameObject Necromancer;
    public GameObject EnemyHealth;
    public GameObject SmallProjectile;
    private const float moveSpeed = 2f;
    private const float viewDistanceMax = 20f;
    private const float viewDistanceMin = 10f;
    private const float baseHealth = 10f;
    private const float healthYOffset = 0.5f;
    private const float shootCooldown = 2f;
    private EnemyHealth healthScript;
    private float cooldown;

    void Start() {
        InstantiateHealth();
    }

    void Update() {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
        if(distanceToPlayer <= viewDistanceMax && distanceToPlayer >= viewDistanceMin) {
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, moveSpeed * Time.deltaTime);
        } else if(distanceToPlayer < viewDistanceMin && cooldown < 0) {
            cooldown = shootCooldown;
            InstantiateSmallProjectile();
        } else {
            cooldown -= Time.deltaTime;
        }
        if(healthScript.Health <= 0) {
            Necromancer.GetComponent<Necromancer>().AliveCount--;
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage) {
        healthScript.Health -= damage;
    }

    void InstantiateHealth() {
        Vector3 healthPosition = new Vector3(
            transform.position.x,
            transform.position.y + healthYOffset,
            transform.position.z
        );
        GameObject healthObject = Instantiate(EnemyHealth, healthPosition, Quaternion.identity, transform);
        healthScript = healthObject.GetComponent<EnemyHealth>();
        healthScript.Enemy = gameObject;
        healthScript.BaseHealth = baseHealth;
        healthScript.DoNotDestroy = false;
    }

    void InstantiateSmallProjectile() {
        GameObject smallProjectile = Instantiate(SmallProjectile, transform.position, Quaternion.identity);
        SmallProjectile smallProjectileScript = smallProjectile.GetComponent<SmallProjectile>();
        smallProjectileScript.playerPosition = Player.transform.position;
    }
}
