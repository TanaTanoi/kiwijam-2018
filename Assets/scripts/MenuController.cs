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
    [SerializeField] private GameObject pausePanel;

    public void SetVolume(float vol) {
        volume = vol;
    }

    public void ShowMainMenu() {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void ShowPauseMenu() {
        menuPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void HideMenus() {
        menuPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void ToggleSettings() {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

}
