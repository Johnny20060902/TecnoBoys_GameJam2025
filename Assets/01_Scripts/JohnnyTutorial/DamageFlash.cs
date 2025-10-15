using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageFlash : MonoBehaviour
{
    public Color flashColor = Color.white;
    public float flashTime = 0.08f;

    Color baseColor;
    SpriteRenderer sr;
    bool running;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    public void DoFlash()
    {
        if (!running) StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        running = true;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = baseColor;
        running = false;
    }
}
