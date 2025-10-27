using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnMonsterAlien : MonoBehaviour
{
    public GameObject MonsterAlienPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;
    public GameObject dialogUI;

    [Header("Control de ataque")]
    public bool enableAttack = false; 
    private bool monsterSpawned = false;

    private GameObject monster; 
    private AlienUmbrax monsterScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !monsterSpawned)
        {
            monsterSpawned = true;

            if (dialogUI != null)
                dialogUI.SetActive(true);

            StartCoroutine(SpawnAndEnterMonster());
        }
    }

    void Update()
    {
        if (enableAttack && monsterScript != null)
        {
            monsterScript.EnableAttack(true);
        }
    }

    IEnumerator SpawnAndEnterMonster()
    {
        monster = Instantiate(MonsterAlienPrefab, spawnPoint.position, Quaternion.identity);
        monsterScript = monster.GetComponent<AlienUmbrax>();

        if (monsterScript != null)
            monsterScript.EnableAttack(false); 

        float elapsed = 0f;
        float duration = 2f;
        Vector3 startPos = spawnPoint.position;
        Vector3 endPos = targetPoint.position;

        while (elapsed < duration)
        {
            monster.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        monster.transform.position = endPos;
    }
}