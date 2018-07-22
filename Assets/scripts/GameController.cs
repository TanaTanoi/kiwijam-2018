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
    public CharacterInput player;
    private CharacterInput playerInput;
    MenuController menuController;

    private bool playing = false;

    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private int enemyRemainFudge = 3;
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
        menuController.UpdateWave(waveNumber);
        enemySpawner.EnemiesToSpawn = waveNumber * enemiesWaveMultiplier;
        // TODO increase the spawn rate with waveNumber
        enemySpawner.Spawning = true;
        waveActive = true;
    }

    private void CheckForWaveEnd() {
        if (!enemySpawner.Spawning && enemySpawner.GetEnemiesRemaining() < enemyRemainFudge) {
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
        menuController.ShowGameMenu();
        player.gameObject.SetActive(true);
        enemySpawner.enabled = true;
        playing = true;
        player.Reset();
    }

    public void CheckDeath() {
        if (!playerInput.Alive()) {
            menuController.ShowGameoverScreen();
            menuController.UpdateScore(waveNumber, playerKills);
            ResetGame();
            playing = false;
            EndWave();
        }
    }

    public void ResumeGame() {
        menuController.ShowGameMenu();
        enemySpawner.Spawning = true;
        player.gameObject.SetActive(true);
        playing = true;
        Time.timeScale = 1;
    }

    public void PauseGame() {
        menuController.ShowPauseMenu();
        enemySpawner.Spawning = false;
        player.gameObject.SetActive(false);
        playing = false;
        Time.timeScale = 0;
    }

    public void LeaveGame() {
        menuController.ShowMainMenu();
        playing = false;
        ResetGame();
    }

    private void ResetGame() {
        player.gameObject.SetActive(false);
        playerInput.RestoreHealth();
        player.transform.position = Vector3.zero;
        enemySpawner.enabled = false;
        enemySpawner.DestroyAll();
        waveNumber = 0;
        playerKills = 0;
        menuController.UpdateWave(waveNumber);
        menuController.UpdateScore(playerKills);
        Time.timeScale = 1;
    }

	public void QuitGame() {
        Application.Quit();
    }
}
