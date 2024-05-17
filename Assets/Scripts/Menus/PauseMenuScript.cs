using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.SceneManagement;
using Zombies;

public class PauseMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public bool isGamePaused = false;
    public static bool isGamePausedStatic;

    [Header("Referencias")]
    public GameObject pauseMenuUI;
    public GameObject reticula;



    private void Awake()
    {

    }

    private void Start()
    {

    }


    public void VolverAlMenu()
    {
        pauseMenuUI.SetActive(false);
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;

        //GameManager.Instance.gameState = GameState.menu;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Sali del juego :(");
        Application.Quit();
    }

    private void Update()
    {
        isGamePausedStatic = isGamePaused;
    }



    public void ContinueGame()
    {
        reticula.SetActive(true);
        //pauseMenuUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        //GameManager.Instance.gameState = GameState.level;
    }

    public void PauseGame()
    {
        reticula.SetActive(false);
        pauseMenuUI.SetActive(true);

        //pauseMenuUI.SetActive(true);

        isGamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

    }



    public void TogglePauseMenu()
    {
        if (isGamePaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }
}

