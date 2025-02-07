using UnityEngine;
using UnityEngine.SceneManagement;  // To load scenes
using UnityEngine.UI;
public class RetryScene : MonoBehaviour
{
    public Button retryButton; // Reference to the Retry Button

    void Start()
    {
        // Ensure the Retry Button is clickable only after the fade-in is complete
        retryButton.onClick.AddListener(ReloadScene);  // Add listener to button click
    }

    void ReloadScene()
    {
        // Reload the current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
