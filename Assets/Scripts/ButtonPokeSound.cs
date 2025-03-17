using UnityEngine;
using Oculus.Interaction;

public class ButtonPokeSound : MonoBehaviour
{
    [Tooltip("Reference to the AudioSource component")]
    [SerializeField]
    private AudioSource audioSource;

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

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not assigned.", this);
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
            // Play the sound
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }
}
