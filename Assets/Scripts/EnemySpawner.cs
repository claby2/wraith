using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject Player;

    public GameObject NecromancerObject;
    private const int necromancerAmount = 15;

    private enum Enemy {
        necromancer
    };

    private const float spawnWidth = 90f;
    private const float spawnHeight = 45f;

    void Start() {
        InstantiateEnemies(Enemy.necromancer, NecromancerObject, necromancerAmount);
    }

    void InstantiateEnemies(Enemy enemy, GameObject enemyPrefab, int amount) {
        for(int i = 0; i < amount; ++i) {
            Vector3 enemyPosition = new Vector3(
                Random.Range(-1 * (spawnWidth / 2), (spawnWidth / 2)),
                Random.Range(-1 * (spawnHeight / 2), (spawnHeight / 2)),
                transform.position.z
            );
            GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, transform);
            switch(enemy) {
                case Enemy.necromancer:
                    Necromancer enemyScript = enemyObject.GetComponent<Necromancer>();
                    enemyScript.Player = Player;
                    break;
            }
        }
    }
}
