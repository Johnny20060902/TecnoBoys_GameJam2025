using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public GameObject deathPanel;

    public GameObject pausePanel;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pausePanel != null)
        {
            if (pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(false);
                Continue();
            }
            else
            {
                pausePanel.SetActive(true);
                Pause();
            }
        }
    }

    public void Die()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Raul_Menu"); 
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Continue()
    {
        Time.timeScale = 1f;

        if (pausePanel != null && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
        }
    }
}
