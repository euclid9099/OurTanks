using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject pauseMenu;

    void Start () {
        Debug.Log("Created Menu script");
    }

    public void PlayGame () 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenMainMenu () 
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame () 
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }

    void Update () {
        // checking Scene buildIndex is used so the pause menu won't be called in the main menu
        // Scene builIndex = 1: Game Scene
        if (SceneManager.GetActiveScene().buildIndex == 1 && Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(GameManager.Instance.getGameIsPaused()) 
            {
                Resume();
            } else 
            {
                Pause();
            }
        }
    }

    public void Resume () 
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.toggleGameIsPaused(); 
    }

    void Pause () 
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.toggleGameIsPaused(); 
    }
}
