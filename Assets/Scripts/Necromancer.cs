using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Necromancer : MonoBehaviour {
    public SpriteRenderer SpriteRenderer;
    public GameObject Player;
    public GameObject Wisp;
    public GameObject EnemyHealth;
    public int AliveCount;
    private const float spawnRange = 5f;
    private const int spawnMaximum = 3;
    private const float spawnCooldown = 3f;
    private const float baseHealth = 100f;
    private const float healthYOffset = 0.7f;
    private EnemyHealth healthScript;
    private float cooldown;

    void Start() {
        InstantiateHealth();
    }

    void Update() {
        Look();
        if(cooldown < 0) {
            cooldown = spawnCooldown;
            if(AliveCount < spawnMaximum) {
                InstantiateWisp();
            }
        }
        cooldown -= Time.deltaTime;
    }

    public void TakeDamage(float damage) {
        healthScript.Health -= damage;
    }

    void Look() {
        if(transform.position.x > Player.transform.position.x) {
            SpriteRenderer.flipX = false;
        } else if(transform.position.x < Player.transform.position.x) {
            SpriteRenderer.flipX = true;
        }
    }

    void InstantiateWisp() {
        Vector3 enemyPosition = new Vector3(
            Random.Range(transform.position.x - (spawnRange / 2), transform.position.x + (spawnRange / 2)),
            Random.Range(transform.position.y - (spawnRange / 2), transform.position.y + (spawnRange / 2)),
            transform.position.z
        );
        GameObject enemyObject = Instantiate(Wisp, enemyPosition, Quaternion.identity, transform);
        Wisp enemyScript = enemyObject.GetComponent<Wisp>();
        enemyScript.Player = Player;
        enemyScript.Necromancer = gameObject;
        AliveCount++;
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
    }
}
