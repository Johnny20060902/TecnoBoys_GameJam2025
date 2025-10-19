using UnityEngine;
using TMPro; // si usas texto TMP

public class PortalActivator : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text uiHint;               // Texto tipo "Presiona E para activar"
    public GameObject portalBluePrefab;   // Prefab del portal azul
    public GameObject portalOrangePrefab; // Prefab del portal naranja

    [Header("Posiciones de aparición")]
    public Transform blueSpawn;
    public Transform orangeSpawn;

    private bool playerInRange = false;
    private bool activated = false;

    void Start()
    {
        if (uiHint != null)
            uiHint.gameObject.SetActive(false); // 🔹 ahora apaga el GameObject completo
    }

    void Update()
    {
        if (playerInRange && !activated && Input.GetKeyDown(KeyCode.E))
        {
            ActivatePortals();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (uiHint != null)
            {
               
                uiHint.gameObject.SetActive(true); // 🔹 activa el GameObject completo
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (uiHint != null)
                uiHint.gameObject.SetActive(false); // 🔹 lo vuelve a apagar
        }
    }

    void ActivatePortals()
    {
        activated = true;
        if (uiHint != null)
            uiHint.gameObject.SetActive(false); // 🔹 oculta el texto tras activarlo

        // Instanciar los portales en las posiciones que tú elijas
        GameObject blue = Instantiate(portalBluePrefab, blueSpawn.position, blueSpawn.rotation);
        GameObject orange = Instantiate(portalOrangePrefab, orangeSpawn.position, orangeSpawn.rotation);

        // Vincularlos automáticamente
        var b = blue.GetComponent<Portal2D>();
        var o = orange.GetComponent<Portal2D>();
        b.linkedPortal = o;
        o.linkedPortal = b;
    }
}
