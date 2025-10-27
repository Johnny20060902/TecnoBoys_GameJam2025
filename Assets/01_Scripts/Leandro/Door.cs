using UnityEngine;

public class Door : MonoBehaviour
{
    // 🔹 Hace que la puerta se oculte (desaparezca visualmente y deje de colisionar)
    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }

    // 🔹 Hace que la puerta reaparezca (vuelve visible y sólida)
    public void CloseDoor()
    {
        gameObject.SetActive(true);
    }
}
