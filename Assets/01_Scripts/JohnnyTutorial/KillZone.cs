using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si es jugador → muerte instantánea
        if (other.CompareTag("Player"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(hs.maxHealth); // mata al jugador
            }
            return;
        }

        // Si es enemigo → eliminar y actualizar spawner si existe
        if (other.CompareTag("Enemy"))
        {
            EnemyTracker tracker = other.GetComponent<EnemyTracker>();
            if (tracker != null)
                Destroy(tracker); // resta al conteo del spawner

            Destroy(other.gameObject);
        }
    }
}
