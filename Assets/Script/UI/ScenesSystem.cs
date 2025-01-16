using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScenesSystem : MonoBehaviour
{
    [SerializeField] private string newScene;
    [SerializeField] private Transform respawnPlayer;
    [SerializeField] private GameObject player;
       
    public GameObject gameOverUI;
    


    public void NewGame()
    {
        SceneManager.LoadScene(newScene);
    }

    public void TurnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Respawn()
    {
        player.transform.position = respawnPlayer.position;
        Time.timeScale = 1f;
        gameOverUI.SetActive(false);
        /*stats.CurrentHealth = stats.maxHealth;*/
    }

    

    public void ExitGame()
    {
        Application.Quit();
    }
}
