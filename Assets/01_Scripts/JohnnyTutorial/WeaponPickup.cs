using UnityEngine;
using TMPro; // <— IMPORTANTE

public class WeaponPickup : MonoBehaviour
{
    public string playerTag = "Player";
    public TMP_Text uiHint; // <- ahora es TMP_Text

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (uiHint != null) uiHint.enabled = true;

        if (Input.GetKeyDown(KeyCode.E))
        {
            var combat = other.GetComponent<PlayerCombatJohnny>();
            if (combat != null) combat.GrantSword();
            if (uiHint != null) uiHint.enabled = false;
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && uiHint != null) uiHint.enabled = false;
    }
}
