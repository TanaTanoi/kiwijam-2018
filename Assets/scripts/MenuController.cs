using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    // serialised for debugging
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private Text killsText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text scoreText;

    public MusicController MusicController;

    public void SetVolume(float vol) {
        MusicController.Volume = vol;
    }

    private void HideAllPanels() {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void ShowMainMenu() {
        HideAllPanels();
        menuPanel.SetActive(true);
    }

    public void ShowPauseMenu() {
        HideAllPanels();
        pausePanel.SetActive(true);
    }

    public void ShowGameMenu() {
        HideAllPanels();
        gamePanel.SetActive(true);
    }

    public void ShowGameoverScreen() {
        HideAllPanels();
        gameoverPanel.SetActive(true);
    }

    public void UpdateScore(int wave, int kills) {
        scoreText.text = "SCORE: " + wave*kills;
    }

    public void UpdateScore(int kills) {
        killsText.text = "KILLS: " + kills;
    }

    public void UpdateWave(int wave) {
        waveText.text = "WAVE: " + wave;
    }



    public void ToggleSettings() {
        instructionsPanel.SetActive(settingsPanel.activeSelf);
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

}
