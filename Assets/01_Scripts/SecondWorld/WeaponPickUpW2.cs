using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUpW2 : MonoBehaviour
{
    public SceneManagerW2 sceneManager; 
    public GameObject thirdWeapon; 
    public KeyCode pickupKey = KeyCode.E;

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

        gameObject.SetActive(false);

        if (sceneManager != null)
        {
            sceneManager.StartEnemySequence();
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
