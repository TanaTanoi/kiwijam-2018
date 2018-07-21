using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float intervalWiggle = 0.2f;
    [SerializeField] private Transform spawnRegion;

    float nextSpawnTime = 0f;

	// Update is called once per frame
	void Update () {
        if (Time.time > nextSpawnTime) {
            nextSpawnTime = Time.time + spawnInterval + Random.Range(-intervalWiggle, intervalWiggle);
            SpawnEnemy();
        }
	}

    void OnDisable() {
        nextSpawnTime = 0f;
        // TODO remove all currently spawned enemies?
    }

    private void SpawnEnemy() {
        Vector3 spawnPosition = new Vector3(spawnRegion.position.x, spawnRegion.position.y, spawnRegion.position.z);
        spawnPosition.x += Random.Range(-(spawnRegion.lossyScale.x/2), spawnRegion.lossyScale.x/2);
        spawnPosition.y += Random.Range(-(spawnRegion.lossyScale.y/2), spawnRegion.lossyScale.y/2);
        spawnPosition.z += Random.Range(-(spawnRegion.lossyScale.z/2), spawnRegion.lossyScale.z/2);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
    }
}
