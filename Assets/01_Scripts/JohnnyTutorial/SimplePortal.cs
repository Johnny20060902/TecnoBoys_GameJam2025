using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimplePortal : MonoBehaviour
{
    public string nextSceneName = "Johnny_FirstWorldLevel2";
    public ScreenFader screenFader; // opcional
    public string message = "Nivel 2 – Próximamente";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(Go());
    }

    IEnumerator Go()
    {
        if (screenFader != null) yield return screenFader.FadeOut(message);
        SceneManager.LoadScene(nextSceneName);
    }
}
