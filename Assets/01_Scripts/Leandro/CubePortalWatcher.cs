using UnityEngine;
 
public class CubePortalWatcher : MonoBehaviour
{
    [HideInInspector] public PlayerGrab grabber;
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (grabber != null &&
            (other.GetComponent<Portal2D>() != null || other.GetComponent<PortalOnlyCube>() != null))
        {
            grabber.DropObject(fromPortal: true);
        }
    }
}
