using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance;
    public bool hasSword;
    public bool hasUnlockedHumanForm;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetHumanFormUnlocked(bool value)
    {
        hasUnlockedHumanForm = value;
        Debug.Log($"🧍 PlayerProgress actualizado: hasUnlockedHumanForm = {hasUnlockedHumanForm}");
    }

    public void SetSwordObtained(bool value)
    {
        hasSword = value;
        Debug.Log($"🗡️ PlayerProgress actualizado: hasSword = {hasSword}");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureExists()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("PlayerProgress");
            Instance = go.AddComponent<PlayerProgress>();
            DontDestroyOnLoad(go);
            Debug.Log("⚙️ PlayerProgress creado automáticamente");
        }
    }
}
