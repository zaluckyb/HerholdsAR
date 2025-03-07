using UnityEngine;
using Oculus.Interaction;

public class ObjectToggleOnPoke : MonoBehaviour
{
    [Tooltip("Array of GameObjects to enable when poked")]
    [SerializeField]
    private GameObject[] objectsToEnable;

    [Tooltip("Array of GameObjects to disable when poked")]
    [SerializeField]
    private GameObject[] objectsToDisable;

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
            // Enable the specified objects
            foreach (var obj in objectsToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            // Disable the specified objects
            foreach (var obj in objectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
