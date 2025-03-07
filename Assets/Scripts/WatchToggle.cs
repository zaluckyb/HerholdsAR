using UnityEngine;
using Oculus.Interaction;

public class WatchToggle : MonoBehaviour
{
    [Tooltip("Array of GameObjects to toggle when poked")]
    [SerializeField]
    private GameObject[] objectsToToggle;

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

        // Subscribe to the StateChanged event
        pokeInteractable.WhenStateChanged += HandlePoke;
    }

    private void OnDestroy()
    {
        if (pokeInteractable != null)
        {
            // Unsubscribe from the StateChanged event
            pokeInteractable.WhenStateChanged -= HandlePoke;
        }
    }

    private void HandlePoke(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            // Toggle the active state of the specified objects
            foreach (var obj in objectsToToggle)
            {
                if (obj != null)
                {
                    obj.SetActive(!obj.activeSelf); // Toggle between enabled and disabled
                }
            }
        }
    }
}
