using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeSceneW2 : MonoBehaviour
{
    public string scene;

    public ScreenFader screenFader;
    public string messageText = "Sigues a Veyra hasta destingir una ciudad a lo lejos";
    public string nextSceneName = "Raul_SecondWorldLevel2";
    // Start is called before the first frame update
    void Start()
    {
        scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && scene == "Raul_SecondWorldLevel1")
        {
            StartCoroutine(TransitionScene());
        }
        else if (collision.gameObject.CompareTag("Player") && scene == "Raul_SecondWorldLevel2")
        {
            messageText = "Entras a la ciudad";
            nextSceneName = "Raul_SecondWorldLevel3";
            StartCoroutine(TransitionScene());
        }
        else if (collision.gameObject.CompareTag("Player") && scene == "Raul_SecondWorldLevel3")
        {
            messageText = "Esperas unos minutos y ves llegar a alguien a lo lejos";
            nextSceneName = "Raul_SecondWorldLevel4";
            StartCoroutine(TransitionScene());
        }
        else if (collision.gameObject.CompareTag("Player") && scene == "Raul_SecondWorldLevel4")
        {
            messageText = "Te diriges hacia la direccion por donde escapó Val’Thar y encuentras su campamento";
            nextSceneName = "Raul_SecondWorldLevel5";
            StartCoroutine(TransitionScene());
        }
    }

    private IEnumerator TransitionScene()
    {
        if (screenFader != null)
        {
            yield return screenFader.FadeOut(messageText);
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
