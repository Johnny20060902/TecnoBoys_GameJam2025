using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(Collider2D))]
public class PlayerGrab : MonoBehaviour
{
    [Header("Configuración")]
    public float grabDistance = 3f;
    public float moveSpeed = 12f;
    public float holdDistance = 1.5f;
    public LayerMask cubeMask;
 
    private Rigidbody2D heldObject;
    private bool isHolding;
    private bool portalDropLock;
    private bool justDroppedFromPortal;
    private Camera cam;
 
    void Awake() => cam = Camera.main;
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isHolding)
                DropObject();
            else
                TryGrabObject();
        }
 
        if (isHolding && heldObject != null && !portalDropLock && !justDroppedFromPortal)
            MoveHeldObject();
    }
 
    void TryGrabObject()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, grabDistance, cubeMask);
 
        if (hit.collider != null && hit.collider.CompareTag("Cube"))
        {
            Rigidbody2D rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                heldObject = rb;
                heldObject.gravityScale = 0;
                heldObject.velocity = Vector2.zero;
                isHolding = true;
                portalDropLock = false;
                justDroppedFromPortal = false;
 
                // 🔹 Vincular el cubo con este PlayerGrab
                CubePortalWatcher watcher = heldObject.GetComponent<CubePortalWatcher>();
                if (watcher == null)
                    watcher = heldObject.gameObject.AddComponent<CubePortalWatcher>();
                watcher.grabber = this;
            }
        }
    }
 
    void MoveHeldObject()
    {
        if (heldObject == null) return;
 
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
 
        Vector2 targetPos = (Vector2)transform.position + dir * holdDistance;
        Vector2 newPos = Vector2.Lerp(heldObject.position, targetPos, moveSpeed * Time.deltaTime);
        heldObject.MovePosition(newPos);
    }
 
    public void DropObject(bool fromPortal = false)
    {
        if (heldObject != null)
        {
            heldObject.gravityScale = 1;
            heldObject.velocity = Vector2.zero;
            heldObject = null;
        }
 
        isHolding = false;
 
        if (fromPortal)
            StartCoroutine(PortalDropCooldown());
    }
 
    private IEnumerator PortalDropCooldown()
    {
        portalDropLock = true;
        justDroppedFromPortal = true;
        yield return new WaitForSeconds(0.1f);
        justDroppedFromPortal = false;
        yield return new WaitForSeconds(0.15f);
        portalDropLock = false;
    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHolding && other != null &&
            (other.GetComponent<Portal2D>() != null || other.GetComponent<PortalOnlyCube>() != null))
        {
            DropObject(fromPortal: true);
            heldObject = null;
            isHolding = false;
        }
    }
 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabDistance);
    }
 
    public bool IsHoldingCube() => isHolding && heldObject != null;
    public Rigidbody2D GetHeldCube() => heldObject;
}