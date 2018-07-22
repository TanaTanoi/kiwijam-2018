using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(MenuController))]

public class GameController : MonoBehaviour {

    public static GameController Instance = null;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject player;
    private CharacterInput playerInput;
    MenuController menuController;

    private bool playing = false;

    [SerializeField] private float timeBetweenWaves = 3f;

    [Tooltip("waveNumber * enemiesWaveMultiplier = number of enemies to spawn")]
    [SerializeField] private int enemiesWaveMultiplier = 50;

    private bool waveActive = false;
    private int waveNumber = 0;
    private float nextWaveTime = 2f;

    private int playerKills = 0;

    void Start() {
        menuController = GetComponent<MenuController>();
        playerInput = player.GetComponent<CharacterInput>();
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
            CheckDeath();
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

    public Transform GetSpawnerTransform() {
        return enemySpawner.transform;
    }

    public void IncrementKills(GameObject enemy) {
        menuController.UpdateScore(++playerKills);
        enemySpawner.Kill(enemy);
    }

    private void StartWave() {
        waveNumber++;
        enemySpawner.EnemiesToSpawn = waveNumber * enemiesWaveMultiplier;
        // TODO increase the spawn rate with waveNumber
        enemySpawner.Spawning = true;
        waveActive = true;
    }

    private void CheckForWaveEnd() {
        if (!enemySpawner.Spawning && enemySpawner.GetEnemiesRemaining() == 0) {
            EndWave();
        }

    }

    private void EndWave() {
        enemySpawner.Spawning = false;
        nextWaveTime = Time.time + timeBetweenWaves;
        waveActive = false;
    }


    public void StartGame() {
        ResetGame();
        menuController.HideMenus();
        player.SetActive(true);
        enemySpawner.enabled = true;
        playing = true;
    }

    public void CheckDeath() {
        if (!playerInput.Alive()) {
            // TODO show score screen
            LeaveGame(); // TODO remove this
            playing = false;
            EndWave();
        }
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
        playerInput.RestoreHealth();
        player.transform.position = Vector3.up;
        enemySpawner.enabled = false;
        enemySpawner.DestroyAll();
        waveNumber = 0;
        playerKills = 0;
    }

	public void QuitGame() {
        Application.Quit();
    }
}
