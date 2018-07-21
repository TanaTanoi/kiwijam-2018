using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(MenuController))]

public class GameController : MonoBehaviour {

    [SerializeField] private EnemySpawner enemySpawner;
    MenuController menuController;

    [SerializeField] private PlayableDirector masterTimeline;
    [SerializeField] private GameObject player;


    void Start() {
        menuController = GetComponent<MenuController>();
        enemySpawner.SetPlayer(player.transform);

        LeaveGame();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }

    private void ResetGame() {
        /*
         * Disable the player
         * clear all enemies
         * reset the wave counter
         * reset the player's health and position
        */
        //player.Reset();
        player.SetActive(false);
        enemySpawner.enabled = false;
        enemySpawner.DestroyAll();

    }

    public void StartGame() {
        ResetGame();
        menuController.HideMenus();
        player.SetActive(true);
        enemySpawner.enabled = true;
        masterTimeline.Play();
    }

    public void ResumeGame() {
        masterTimeline.Resume();
        menuController.HideMenus();
        enemySpawner.gameObject.SetActive(true);
    }

    public void PauseGame() {
        masterTimeline.Pause();
        menuController.ShowPauseMenu();
        enemySpawner.gameObject.SetActive(false);
    }

    public void LeaveGame() {
        menuController.ShowMainMenu();
        masterTimeline.Stop();
        ResetGame();
    }

	public void QuitGame() {
        Application.Quit();
    }
}
