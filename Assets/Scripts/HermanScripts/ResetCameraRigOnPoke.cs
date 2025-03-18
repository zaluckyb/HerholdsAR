using UnityEngine;
using Oculus.Interaction;

public class ResetCameraRigOnPoke : MonoBehaviour
{
    [Tooltip("The camera rig that will be reset")]
    [SerializeField]
    private Transform cameraRig;

    [Tooltip("Reference to the PokeInteractable component")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    private void Start()
    {
        // Validate required objects
        if (pokeInteractable == null)
        {
            Debug.LogError("PokeInteractable component not assigned.", this);
            return;
        }

        if (cameraRig == null)
        {
            Debug.LogError("Camera Rig not assigned.", this);
            return;
        }

        // Subscribe to poke event
        pokeInteractable.WhenStateChanged += HandlePoke;
    }

    private void OnDestroy()
    {
        if (pokeInteractable != null)
        {
            pokeInteractable.WhenStateChanged -= HandlePoke;
        }
    }

    private void HandlePoke(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            ResetCameraRig();
        }
    }

    private void ResetCameraRig()
    {
        cameraRig.position = Vector3.zero;
        cameraRig.rotation = Quaternion.Euler(0f, 180f, 0f); // Sets Y rotation to 180 degrees
    }
}
