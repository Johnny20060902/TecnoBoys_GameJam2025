using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBattleManageW2 : MonoBehaviour
{
    public GameObject wallNpc;
    public GameObject spawn;
    void Update()
    {
        if (spawn.activeInHierarchy)
        {
            int enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (enemiesLeft == 0)
            {
                wallNpc.SetActive(false);
            }
            else
            {
                wallNpc.SetActive(true);
            }
        }

    }
}
