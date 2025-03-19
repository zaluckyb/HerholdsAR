using UnityEngine;
using Oculus.Interaction;

public class MenuToggle : MonoBehaviour
{
    [Tooltip("Array of GameObjects to toggle when poked")]
    [SerializeField]
    private GameObject[] objectsToToggle;

    [Tooltip("Reference to the PokeInteractable component")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    [Tooltip("Reference to the XR Rig's Center Eye Anchor (Camera)")]
    [SerializeField]
    private Transform centerEyeAnchor;

    [Tooltip("Distance in front of the user to place the menu")]
    [SerializeField]
    private float menuDistance = 0.6f;

    [Tooltip("Height offset relative to the eye level")]
    [SerializeField]
    private float heightOffset = -0.1f;

    private Quaternion initialRotation; // Store the menu's initial rotation

    private void Start()
    {
        if (pokeInteractable == null)
        {
            Debug.LogError("PokeInteractable component not assigned.", this);
            return;
        }

        if (centerEyeAnchor == null)
        {
            Debug.LogError("Center Eye Anchor is not assigned!", this);
            return;
        }

        if (objectsToToggle.Length > 0 && objectsToToggle[0] != null)
        {
            // Store the initial rotation of the first menu object
            initialRotation = objectsToToggle[0].transform.rotation;
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
            foreach (var obj in objectsToToggle)
            {
                if (obj != null)
                {
                    bool isActive = !obj.activeSelf;
                    obj.SetActive(isActive);

                    if (isActive)
                    {
                        MoveMenuInFrontOfUser(obj);
                    }
                }
            }
        }
    }

    private void MoveMenuInFrontOfUser(GameObject menu)
    {
        if (centerEyeAnchor == null) return;

        Vector3 forwardDirection = centerEyeAnchor.forward;
        forwardDirection.y = 0; // Keep the menu level
        forwardDirection.Normalize();

        Vector3 newPosition = centerEyeAnchor.position + forwardDirection * menuDistance;
        newPosition.y += heightOffset;

        // Reset position and apply the original rotation
        menu.transform.position = newPosition;
        menu.transform.rotation = initialRotation; // Reset rotation to original
    }
}
