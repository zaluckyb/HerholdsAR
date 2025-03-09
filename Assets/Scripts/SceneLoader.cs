using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextScene; // Set this in the Inspector

    void Start()
    {
        // Ensure nextScene is not empty
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set in the inspector.");
        }
    }
}
