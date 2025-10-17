using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp = 3f;
    [HideInInspector] public float hp;

    public float moveSpeed = 2.5f;
    public Transform target;

    void Awake()
    {
        // si hp no está seteado, igualarlo al máximo
        if (hp <= 0) hp = maxHp;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(float d)
    {
        hp -= d;
        GetComponent<DamageFlash>()?.DoFlash();
        if (hp <= 0) Destroy(gameObject);
    }
}
