using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerW2 : MonoBehaviour
{
    [Header("Enemigos a eliminar")]
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Objeto de recompensa")]
    public GameObject rewardObject;

    private bool rewardGiven = false;

    void Start()
    {
        if (rewardObject != null)
            rewardObject.SetActive(false);
    }

    void Update()
    {
        if (rewardGiven) return;

        enemies.RemoveAll(e => e == null);

        if (enemies.Count == 0)
        {
            ActivateReward();
        }
    }

    void ActivateReward()
    {
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            rewardGiven = true;
        }
    }
}
