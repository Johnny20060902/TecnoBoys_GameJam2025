using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSoldiers : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject soldierAlien;
    public GameObject soldierAlienShoot;
    public GameObject soldierStrong;
    public Transform InitialPositionSpawn;
    public Transform FinalPositionSpawn;

    public bool spawnBasicEnemies;
    public bool spawnBasicShootEnemies;
    public bool strongEnemies;

    void Start()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_SecondWorldLevel4")
        {
            SpawnSoldiersCity();
        }

        if (scene == "Raul_SecondWorldLevel5" )
        {
            SpawnSoldiersFort();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnSoldiersCity()
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < 4)
            {
                Instantiate(soldierAlien, new Vector2(InitialPositionSpawn.position.x, Random.Range(InitialPositionSpawn.position.y, FinalPositionSpawn.position.y)), Quaternion.identity);
            }
            else
            {
                Instantiate(soldierAlienShoot, new Vector2(InitialPositionSpawn.position.x, Random.Range(InitialPositionSpawn.position.y, FinalPositionSpawn.position.y)), Quaternion.identity);
            }
        }
    }



    void SpawnSoldiersFort()
    {
        GameObject enemyObj;
        if (spawnBasicEnemies)
        {
            for (int i = 0; i < 3; i++)
            {
                enemyObj = Instantiate(soldierAlien, new Vector2(gameObject.transform.position.x , gameObject.transform.position.y + i), Quaternion.identity);
                SoldierAlien enemyScript = enemyObj.GetComponent<SoldierAlien>();
                enemyScript.EnableAttack(true);
            }

        }
        else if (spawnBasicShootEnemies)
        {
            for (int i = 0; i < 3; i++)
            {
                enemyObj = Instantiate(soldierAlienShoot, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + i), Quaternion.identity);
                SoldierAlienWithGun enemyScript = enemyObj.GetComponent<SoldierAlienWithGun>();
                enemyScript.EnableAttack(true);
            }

        }
        else if (strongEnemies)
        {
            for (int i = 0; i < 2; i++)
            {
                enemyObj = Instantiate(soldierStrong, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + i), Quaternion.identity);
                SoldierAlienStrong enemyScript = enemyObj.GetComponent<SoldierAlienStrong>();
                enemyScript.EnableAttack(true);
            }
        }

    }
}
