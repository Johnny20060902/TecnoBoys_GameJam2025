using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneSaveData
{
    public string sceneName;
}

public class PlayerSceneSave : MonoBehaviour
{
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "sceneSave.json");
        Debug.Log("Ruta de guardado: " + savePath);
    }

    void Start()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene != "Raul_Menu")
        {
            SaveScene();
        }

    }

    // Guardar la escena actual
    public void SaveScene()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("savePath no está inicializado");
            return;
        }

        SceneSaveData data = new SceneSaveData();
        data.sceneName = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);

        Debug.Log("Escena guardada: " + data.sceneName);
    }

    // Cargar la escena guardada
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("savePath no está inicializado");
            return;
        }

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SceneSaveData data = JsonUtility.FromJson<SceneSaveData>(json);

            SceneManager.LoadScene(data.sceneName);
            Debug.Log("Escena cargada: " + data.sceneName);
        }
        else
        {
            Debug.Log("No hay escena guardada");
        }
    }
}
