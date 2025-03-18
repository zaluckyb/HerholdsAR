using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Interaction;

public class LoadSceneOnPoke : MonoBehaviour
{
    [Tooltip("Name of the scene to load when poked")]
    [SerializeField]
    private string sceneToLoad;

    [Tooltip("Reference to the PokeInteractable component")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    private void Start()
    {
        if (pokeInteractable == null)
        {
            Debug.LogError("PokeInteractable component not assigned.", this);
            return;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Scene name not assigned.", this);
            return;
        }

        // Subscribe to the Select event
        pokeInteractable.WhenStateChanged += HandlePoke;
    }

    private void OnDestroy()
    {
        if (pokeInteractable != null)
        {
            // Unsubscribe from the Select event
            pokeInteractable.WhenStateChanged -= HandlePoke;
        }
    }

    private void HandlePoke(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
