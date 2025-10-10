using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string nextSceneName = "Raul_Introduction";
    public float absorbDuration = 1f;
    public ScreenFader screenFader;
    public string messageText = "De Pronto Despiertas en un Lugar Desconocido...";
    public float absorbRange = 20f; 

    private bool isActivated = false;

    private void Update()
    {
        if (isActivated) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= absorbRange)
            {
                isActivated = true;
                StartCoroutine(ActivatePortal(player.transform));
            }
        }
    }

    private IEnumerator ActivatePortal(Transform player)
    {
        float timer = 0f;
        Vector3 startPos = player.position;
        Vector3 endPos = transform.position;

        while (timer < absorbDuration)
        {
            player.position = Vector3.Lerp(startPos, endPos, timer / absorbDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        player.position = endPos;

        if (screenFader != null)
        {
            yield return screenFader.FadeOut(messageText);
        }

        SceneManager.LoadScene(nextSceneName);
    }
}