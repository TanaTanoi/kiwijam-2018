using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float intervalWiggle = 0.2f;
    [SerializeField] private bool playing = true; // TODO move this to gamecontroller
    [SerializeField] private Transform spawnRegion;


	// Update is called once per frame
	void Start () {
        StartCoroutine("SpawnEnemy");
	}

    public IEnumerator SpawnEnemy() {
        while(playing) {
            Vector3 spawnPosition = new Vector3(spawnRegion.position.x, spawnRegion.position.y, spawnRegion.position.z);
            spawnPosition.x += Random.Range(-(spawnRegion.lossyScale.x/2), spawnRegion.lossyScale.x/2);
            spawnPosition.y += Random.Range(-(spawnRegion.lossyScale.y/2), spawnRegion.lossyScale.y/2);
            spawnPosition.z += Random.Range(-(spawnRegion.lossyScale.z/2), spawnRegion.lossyScale.z/2);

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);

            float delay = spawnInterval + Random.Range(-intervalWiggle, intervalWiggle);
            yield return new WaitForSeconds(delay);
        }
    }
}
