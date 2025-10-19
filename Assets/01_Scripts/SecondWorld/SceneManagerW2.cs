using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerW2 : MonoBehaviour
{
    public Player player;
    public ValThar enemy;
    public Transform enemyStartPos;
    public Transform enemyTargetPos;
    public GameObject sceneToActivate;

    private bool enemyMoving = false;

    void Start()
    {
        player.EnableMovement(true);
    }

    public void StartEnemySequence()
    {
        player.EnableMovement(false);

        enemy.transform.position = enemyStartPos.position;

        enemyMoving = true;
    }

    void Update()
    {
        if (!enemyMoving) return;

        enemy.transform.position = Vector2.MoveTowards(
            enemy.transform.position,
            enemyTargetPos.position,
            enemy.moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(enemy.transform.position, enemyTargetPos.position) < 0.1f)
        {
            enemyMoving = false;
            player.EnableMovement(true); 
            sceneToActivate.SetActive(true);
        }
    }
}
