using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSIMPLE : MonoBehaviour
{
    public int totalLevels = 6; // Cambiado a tus 6 niveles
    public ScreenFader screenFader;
    public string transitionMessage = "Cargando siguiente nivel...";
    private bool isTransitioning = false;

    void OnTriggerEnter2D(Collider2D other)
{
    if (isTransitioning) return;

    // Busca el Player en el objeto raíz por si el collider está en un hijo
    var player = other.GetComponentInParent<PlayerLeandro>();
    if (player == null) return;

    Debug.Log("✅ Player entró al portal");
    StartCoroutine(GoToNextLevel());
}


    IEnumerator GoToNextLevel()
    {
        isTransitioning = true;

        string currentScene = SceneManager.GetActiveScene().name;
        int currentLevel = GetCurrentLevel(currentScene);
        int nextLevel = Mathf.Clamp(currentLevel + 1, 1, totalLevels);
        string nextSceneName = $"Leandro_ThirdWorldLevel{nextLevel}";

        if (screenFader != null)
            yield return screenFader.FadeOut(transitionMessage);

        SceneManager.LoadScene(nextSceneName);
    }

    int GetCurrentLevel(string sceneName)
    {
        // Busca el número al final del nombre (Level1 → 1, Level2 → 2, etc.)
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
