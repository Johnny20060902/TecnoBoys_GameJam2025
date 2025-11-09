using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextW2 : MonoBehaviour
{
    public static TutorialTextW2 Instance;

    [Header("Referencia al texto del tutorial")]
    public TextMeshProUGUI tutorialText;

    [Header("Tiempo que se mantiene visible el texto")]
    public float displayTime = 4f;

    private Coroutine currentRoutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (tutorialText != null)
            tutorialText.text = "";
    }

    /// <summary>
    /// Muestra un mensaje temporal en pantalla.
    /// </summary>
    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private System.Collections.IEnumerator ShowMessageRoutine(string message)
    {
        tutorialText.text = message;
        tutorialText.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        float fadeTime = 1f;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            tutorialText.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        tutorialText.text = "";
    }
}
