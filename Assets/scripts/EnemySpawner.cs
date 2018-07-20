using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float intervalWiggle = 0.2f;
    [SerializeField] private bool playing = true; // TODO move this to gamecontroller

	// Update is called once per frame
	void Start () {
        StartCoroutine("SpawnEnemy");
	}

    public IEnumerator SpawnEnemy() {
        while(playing) {
            // TODO spawn in region
            Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, transform);

            float delay = spawnInterval + Random.Range(-intervalWiggle, intervalWiggle);
            print(delay);
            yield return new WaitForSeconds(delay);
        }
    }
}
