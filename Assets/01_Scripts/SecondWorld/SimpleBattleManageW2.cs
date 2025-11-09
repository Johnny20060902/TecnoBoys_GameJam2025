using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBattleManageW2 : MonoBehaviour
{
    public GameObject wallNpc;
    public GameObject spawn;
    void Update()
    {
        if (spawn.activeInHierarchy)
        {
            int enemiesLeft = GameObject.FindGameObjectsWithTag("SoldierAlien").Length + GameObject.FindGameObjectsWithTag("SoldierStrongAlien").Length + GameObject.FindGameObjectsWithTag("SoldierGunAlien").Length;

            if (enemiesLeft == 0)
            {
                wallNpc.SetActive(false);

                string sceneName = SceneManager.GetActiveScene().name;

                if (sceneName == "Raul_SecondWorldLevel3")
                {
                    TutorialTextW2.Instance.ShowMessage("Habla con el aldeano asustado");
                }
                else if (sceneName == "Raul_SecondWorldLevel4")
                {
                    TutorialTextW2.Instance.ShowMessage("Habla con el aldeano dentro de la ciudad");
                }
                else
                {
                    TutorialTextW2.Instance.ShowMessage("");
                }
            }
            else
            {
                wallNpc.SetActive(true);
            }
        }

    }
}
