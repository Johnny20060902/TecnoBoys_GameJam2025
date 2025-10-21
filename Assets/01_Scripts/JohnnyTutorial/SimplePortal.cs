using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimplePortal : MonoBehaviour
{
    public int totalLevels = 5;
    public ScreenFader screenFader;
    public string transitionMessage = "Cargando siguiente nivel...";
    private bool isTransitioning = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isTransitioning) return;
        StartCoroutine(GoToNextLevel());
    }

    IEnumerator GoToNextLevel()
    {
        isTransitioning = true;

        string currentScene = SceneManager.GetActiveScene().name;
        int currentLevel = GetCurrentLevel(currentScene);
        int nextLevel = Mathf.Clamp(currentLevel + 1, 1, totalLevels);
        string nextSceneName = $"Johnny_FirstWorldLevel{nextLevel}";

        if (screenFader != null)
            yield return screenFader.FadeOut(transitionMessage);

        SceneManager.LoadScene(nextSceneName);
    }

    int GetCurrentLevel(string sceneName)
    {
        // Extrae el número al final del nombre (ejemplo: Level1 → 1)
        for (int i = sceneName.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(sceneName[i]))
            {
                string num = sceneName[i].ToString();
                return int.Parse(num);
            }
        }
        return 1;
    }
}
