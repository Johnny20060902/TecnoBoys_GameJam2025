using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class IntroText : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] storyLines;        
    public float typingSpeed = 0.03f;  
    public Text storyText;           
    public float delayBetweenLines = 2f; 
    public string nextScene = "Raul_Introduction"; 

    private void Start()
    {
        StartCoroutine(ShowStory());
    }

    IEnumerator ShowStory()
    {
        storyText.text = "";
        yield return new WaitForSeconds(1f);

        foreach (string line in storyLines)
        {
            storyText.text = "";
            foreach (char c in line)
            {
                storyText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(delayBetweenLines);
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextScene);
    }
}
