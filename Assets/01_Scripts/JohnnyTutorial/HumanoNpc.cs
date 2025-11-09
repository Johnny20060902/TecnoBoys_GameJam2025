using UnityEngine;

public class HumanoNpc : MonoBehaviour
{
    public GameObject textoIndicador;
    private bool jugadorCerca = false;

    private void Start()
    {
        if (textoIndicador != null)
            textoIndicador.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (textoIndicador != null)
                textoIndicador.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (textoIndicador != null)
                textoIndicador.SetActive(false);
        }
    }

    public bool EstaJugadorCerca()
    {
        return jugadorCerca;
    }
}
