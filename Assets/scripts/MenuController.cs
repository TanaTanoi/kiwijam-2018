﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    // serialised for debugging
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text killsText;

    public MusicController MusicController;

    public void SetVolume(float vol) {
        MusicController.Volume = vol;
    }

    public void ShowMainMenu() {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void ShowPauseMenu() {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void HideMenus() {
        menuPanel.SetActive(false);
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void UpdateScore(int kills) {
        killsText.text = "Kills: " + kills;
    }

    public void ToggleSettings() {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

}
