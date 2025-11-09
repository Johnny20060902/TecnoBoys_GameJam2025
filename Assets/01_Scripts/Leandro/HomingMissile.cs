using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header("Movimiento")]
    public float ascentSpeed = 5f;
    public float descentSpeed = 6f;
    public float fallDelay = 1.5f;

    [Header("Colisiones")]
    public LayerMask hitMask; // asigna Player y Ground aquí

    public int damage = 25;

    private float timer = 0f;
    private bool falling = false;
    private Transform target;

    public void SetTarget(Transform t) => target = t; // NO usa tag

    private void Update()
    {
        timer += Time.deltaTime;

        if (!falling)
        {
            transform.Translate(Vector2.up * ascentSpeed * Time.deltaTime, Space.Self);
            if (timer >= fallDelay) falling = true;
        }
        else
        {
            if (target)
            {
                Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
                transform.position += (Vector3)(dir * descentSpeed * Time.deltaTime);
                float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, ang), Time.deltaTime * 4f);
            }
            else
            {
                transform.Translate(Vector2.down * descentSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            Destroy(gameObject);
        }
    }
}
