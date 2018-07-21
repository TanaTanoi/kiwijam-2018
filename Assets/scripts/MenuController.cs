using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    // serialised for debugging
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;

    public void SetVolume(float vol) {
        volume = vol;
    }

    void Start() {
        ShowMenu(true);
    }

    public void ShowMenu(bool active) {
        // TODO, start or stop the game, tell the game controller to reset
        //       the game state.

        menuPanel.SetActive(active);
        if (active) {
            Time.timeScale = 0f;
        }
        else {
            Time.timeScale = 1f;
        }
    }

    public void ToggleSettings() {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
