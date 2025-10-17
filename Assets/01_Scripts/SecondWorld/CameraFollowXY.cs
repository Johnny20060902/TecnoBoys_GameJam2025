using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowXY : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Follow Axes")]
    public bool followX = true;
    public bool followY = true;

    [Header("Offset & Bounds")]
    public Vector3 offset = new Vector3(5, 3, -10);
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 desiredPosition;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.1f;

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;

        desiredPosition = new Vector3(
            followX ? target.position.x : pos.x,
            followY ? target.position.y : pos.y,
            offset.z
        ) + new Vector3(offset.x, offset.y, 0);

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
