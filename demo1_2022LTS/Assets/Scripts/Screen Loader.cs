using UnityEngine;
using UnityEngine.SceneManagement; // Crucial for scene operations

public class SceneLoader : MonoBehaviour
{
    // A string to hold the name of the scene we want to load next.
    public string nextSceneName = "MainMenu"; // Default name, change in Inspector!

    void Start()
    {
        // Load the next scene a short time after the game starts, 
        // to give systems time to initialize.
        // Or, you could call this method from a button press.
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        // Check if the scene name is valid before attempting to load
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // The main command to load a scene by its name
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("The next scene name is not set in the Inspector!");
        }
    }

    // Alternative: Load a scene by its index in the Build Settings
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}