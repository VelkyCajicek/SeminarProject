using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject pauseMenu;
    public GameObject SettingsMenu;
    public AudioSource Source; // For lowering the audio once game is paused
    public void Update()
    {
        if(pauseMenu.gameObject.activeSelf == false)
        {
            ContinueGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused && SettingsMenu.activeSelf == false)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }
    public void ContinueGame()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
        isGamePaused = false;
    }
    public void QuitGame()
    {
        SceneManager.LoadSceneAsync(0); 
    }
}
