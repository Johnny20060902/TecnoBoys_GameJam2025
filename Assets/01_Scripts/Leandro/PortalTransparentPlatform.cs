using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plataforma sólida para el jugador, pero que permite que los disparos de portales pasen a través.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PortalTransparentPlatform : MonoBehaviour
{
    [Header("Configuración de capas")]
    [Tooltip("Capa del jugador y cubos que deben colisionar con la plataforma.")]
    public LayerMask solidLayers;

    [Tooltip("Capa usada por los rayos del portal (para ignorar colisión).")]
    public LayerMask portalRayLayer;

    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = false; // 🔹 Es sólida normalmente

        // 🔹 Asegurar que tenga un tag para detección si lo necesitás
        if (string.IsNullOrEmpty(gameObject.tag))
            gameObject.tag = "TransparentPlatform";
    }

    // 🔹 Evita que los rayos de portales choquen con la plataforma
    void OnCollisionEnter2D(Collision2D collision)
    {
        int otherLayer = collision.gameObject.layer;

        // Si el objeto que colisiona pertenece al "portal ray layer", se ignora la colisión
        if (((1 << otherLayer) & portalRayLayer) != 0)
        {
            Physics2D.IgnoreCollision(collision.collider, col, true);
        }
    }
}
