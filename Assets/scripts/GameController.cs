using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(MenuController))]

public class GameController : MonoBehaviour {

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject player;
    MenuController menuController;

    private bool playing = false;

    [SerializeField] private float timeBetweenWaves = 3f;

    [Tooltip("waveNumber * enemiesWaveMultiplier = number of enemies to spawn")]
    [SerializeField] private int enemiesWaveMultiplier = 50;

    private bool waveActive = false;
    private int waveNumber = 0;
    private float nextWaveTime = 2f;

    void Start() {
        menuController = GetComponent<MenuController>();
        enemySpawner.SetPlayer(player.transform);
        LeaveGame();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (playing) {
                PauseGame();
            }
            else {
                ResumeGame();
            }
        }

        if (playing) {
            if (waveActive) {
                // End of wave check
                CheckForWaveEnd();
            }
            else {
                if (Time.time >= nextWaveTime) {
                    StartWave();
                }
            }
        }
    }

    private void StartWave() {
        waveNumber++;
        print("Starting wave " + waveNumber);
        enemySpawner.EnemiesToSpawn = waveNumber * enemiesWaveMultiplier;
        // TODO increase the spawn rate with waveNumber
        enemySpawner.Spawning = true;
        waveActive = true;
    }

    private void CheckForWaveEnd() {
        if (!enemySpawner.Spawning && enemySpawner.GetEnemiesRemaining() == 0) {
            print("Ending wave " + waveNumber);
            enemySpawner.Spawning = false;
            nextWaveTime = Time.time + timeBetweenWaves;
            waveActive = false;
        }
    }




    public void StartGame() {
        ResetGame();
        menuController.HideMenus();
        player.SetActive(true);
        enemySpawner.enabled = true;
        playing = true;
    }

    public void ResumeGame() {
        menuController.HideMenus();
        enemySpawner.Spawning = true;
        player.SetActive(true);
        playing = true;
    }

    public void PauseGame() {
        menuController.ShowPauseMenu();
        enemySpawner.Spawning = false;
        player.SetActive(false);
        playing = false;
    }

    public void LeaveGame() {
        menuController.ShowMainMenu();
        playing = false;
        ResetGame();
    }

    private void ResetGame() {
        player.SetActive(false);
        player.transform.position = Vector3.up;
        enemySpawner.enabled = false;
        enemySpawner.DestroyAll();
        waveNumber = 0;
    }

	public void QuitGame() {
        Application.Quit();
    }
}
