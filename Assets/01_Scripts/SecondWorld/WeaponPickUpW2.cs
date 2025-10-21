using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUpW2 : MonoBehaviour
{
    public SceneManagerW2 sceneManager; 
    public GameObject thirdWeapon; 
    public KeyCode pickupKey = KeyCode.E;
    public GameObject wallNotEnemy;

    private bool isInRange = false;

    void Update()
    {
        if (isInRange && Input.GetKeyDown(pickupKey))
        {
            PickupWeapon();
        }
    }

    void PickupWeapon()
    {
        Debug.Log("Arma recogida");

        if (thirdWeapon != null)
            thirdWeapon.SetActive(true);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.UnlockThirdWeapon();
            }
        }

        gameObject.SetActive(false);

        if (sceneManager != null)
        {
            sceneManager.StartEnemySequence();
            if (wallNotEnemy != null)
            {
                wallNotEnemy.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

}
