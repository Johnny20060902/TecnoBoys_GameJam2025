using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSoldiers : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject soldierAlien;
    public GameObject soldierAlienShoot;
    public Transform InitialPositionSpawn;
    public Transform FinalPositionSpawn;
    void Start()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_SecondWorldLevel4")
        {
            SpawnSoldiersCity();
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
}
