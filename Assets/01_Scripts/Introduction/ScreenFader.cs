using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;      
    public Text messageTextUI;    
    public float fadeDuration = 1f;

    private void Awake()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        messageTextUI.text = "";
    }

    public IEnumerator FadeOut(string message)
    {
        messageTextUI.text = message;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(1f); 
    }
}
