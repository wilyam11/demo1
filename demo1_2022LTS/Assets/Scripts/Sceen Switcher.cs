using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // This function will be called by the Button's OnClick event.
    public void LoadSceneByName(string sceneName)
    {
        // Check if the scene name is valid (optional but good practice)
        if (!string.IsNullOrEmpty(sceneName))
        {
            // The main command to load a scene by its exact name
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not specified!");
        }
    }
}