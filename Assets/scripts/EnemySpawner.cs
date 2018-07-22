using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float intervalWiggle = 0.2f;
    [SerializeField] private Transform spawnRegion;

    [HideInInspector] public bool Spawning = false;
    [HideInInspector] public int EnemiesToSpawn = 0;

    private float nextSpawnTime = 0f;
    private Transform enemyTarget;

    List<GameObject> enemyPool = new List<GameObject>();

    public void SetPlayer(Transform target) {
        this.enemyTarget = target;
    }

	// Update is called once per frame
	void Update () {
        if (Spawning) {
            if (enemyTarget != null) {
                if (Time.time > nextSpawnTime) {
                    nextSpawnTime = Time.time + spawnInterval + Random.Range(-intervalWiggle, intervalWiggle);
                    SpawnEnemy();
                    EnemiesToSpawn--;
                    if (EnemiesToSpawn <= 0) {
                        Spawning = false;
                    }
                }
            }
            else {
                Debug.LogWarning("Attempted to spawn enemies while there is no player, disabling spawning.");
                Spawning = false;
            }
        }
	}

    public void Kill(GameObject enemy) {
        if(!enemyPool.Remove(enemy)) {
            Debug.Log("Failed to remove enemy");
        }
    }

    public int GetEnemiesRemaining() {
        return enemyPool.Count;
    }

    public void DestroyAll() {
        foreach (GameObject e in enemyPool) {
            Destroy(e);
        }
        enemyPool.Clear();
    }

    private void SpawnEnemy() {
        Vector3 spawnPosition = new Vector3(spawnRegion.position.x, spawnRegion.position.y, spawnRegion.position.z);
        spawnPosition.x += Random.Range(-(spawnRegion.lossyScale.x/2), spawnRegion.lossyScale.x/2);
        spawnPosition.y += Random.Range(-(spawnRegion.lossyScale.y/2), spawnRegion.lossyScale.y/2);
        spawnPosition.z += Random.Range(-(spawnRegion.lossyScale.z/2), spawnRegion.lossyScale.z/2);

        GameObject instance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
        instance.GetComponent<Enemy>().Spawn(enemyTarget);
        enemyPool.Add(instance);
    }
}
