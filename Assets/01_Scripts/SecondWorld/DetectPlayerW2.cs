using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerW2 : MonoBehaviour
{
    public GameObject Spawn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Spawn != null)
            {
                Spawn.SetActive(true);
            }

        }
    }
}
