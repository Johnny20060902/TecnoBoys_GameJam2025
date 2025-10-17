using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("Configuración")]
    public float grabDistance = 3f;       // distancia máxima para agarrar
    public float moveSpeed = 12f;         // velocidad de movimiento del cubo
    public float holdDistance = 1.5f;     // distancia fija a la que el cubo se mantiene del jugador
    public LayerMask cubeMask;            // capa o tag de los cubos

    private Rigidbody2D heldObject;
    private bool isHolding;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHolding)
                DropObject();
            else
                TryGrabObject();
        }

        if (isHolding && heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void TryGrabObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        // raycast hacia el mouse
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, grabDistance, cubeMask);
        if (hit.collider != null && hit.collider.CompareTag("Cube"))
        {
            heldObject = hit.collider.attachedRigidbody;
            if (heldObject != null)
            {
                heldObject.gravityScale = 0;
                heldObject.velocity = Vector2.zero;
                isHolding = true;
            }
        }
    }

    void MoveHeldObject()
    {
        // posición del mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // dirección desde el jugador al mouse
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        // posición destino: frente al jugador, pero a distancia fija
        Vector2 targetPos = (Vector2)transform.position + dir * holdDistance;

        // interpolación suave para que el cubo se mueva natural
        Vector2 newPos = Vector2.Lerp(heldObject.position, targetPos, moveSpeed * Time.deltaTime);
        heldObject.MovePosition(newPos);
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.gravityScale = 1;
            heldObject = null;
        }
        isHolding = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabDistance);
    }

    public bool IsHoldingCube()
    {
        return isHolding && heldObject != null;
    }

    public Rigidbody2D GetHeldCube()
    {
        return heldObject;
    }
}
