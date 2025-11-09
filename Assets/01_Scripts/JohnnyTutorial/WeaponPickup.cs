using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponPickup : MonoBehaviour
{
    public string playerTag = "Player";
    public Canvas uiHint;

    private bool playerInRange;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;

        if (uiHint != null)
            uiHint.enabled = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;

        if (uiHint != null)
            uiHint.enabled = false;
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        bool isLevel1 = sceneName == "Johnny_FirstWorldLevel1";

        // 🧠 Solo permitir recoger en el nivel 1
        if (!isLevel1) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🗝️ Presionaste E dentro del rango del pickup");

            // Buscar al jugador correctamente (incluso si el collider es hijo)
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj == null)
            {
                Debug.LogWarning("❌ No se encontró objeto con tag Player");
                return;
            }

            PlayerCombatJohnny combat = playerObj.GetComponent<PlayerCombatJohnny>();
            if (combat == null)
            {
                combat = playerObj.GetComponentInChildren<PlayerCombatJohnny>();
                if (combat == null)
                    combat = playerObj.GetComponentInParent<PlayerCombatJohnny>();
            }

            if (combat != null)
            {
                combat.GrantSword();

                if (PlayerProgress.Instance != null)
                    PlayerProgress.Instance.SetSwordObtained(true);

                if (uiHint != null)
                    uiHint.enabled = false;

                Destroy(gameObject);
                Debug.Log("✅ Espada recogida correctamente con tecla E en Nivel 1");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró el componente PlayerCombatJohnny en el jugador");
            }
        }
    }
}
