using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Raul_IntroScene");
    }

    public void Continue()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
