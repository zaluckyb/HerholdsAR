using UnityEngine;
using Oculus.Interaction;
using System.Collections;

public class ScaleOnPoke : MonoBehaviour
{
    [Tooltip("Root object to be scaled")]
    [SerializeField]
    private Transform rootObject;

    [Tooltip("Pivot point to which the root object aligns before scaling")]
    [SerializeField]
    private Transform pivotPoint;

    [Tooltip("Target scale when exploded")]
    [SerializeField]
    private Vector3 targetScale = new Vector3(2f, 2f, 2f);

    [Tooltip("Scaling speed (higher means faster scale transition)")]
    [SerializeField]
    private float scaleSpeed = 2f;

    [Tooltip("Reference to the PokeInteractable component")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isScaled = false;

    private void Start()
    {
        // Validate required objects
        if (pokeInteractable == null)
        {
            Debug.LogError("PokeInteractable component not assigned.", this);
            return;
        }

        if (rootObject == null)
        {
            Debug.LogError("Root object not assigned.", this);
            return;
        }

        if (pivotPoint == null)
        {
            Debug.LogError("Pivot point not assigned.", this);
            return;
        }

        // Store original scale, position, and rotation
        originalScale = rootObject.localScale;
        originalPosition = rootObject.position;
        originalRotation = rootObject.rotation;

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
            if (!isScaled)
            {
                StartCoroutine(ScaleAndMoveObject(pivotPoint.position, pivotPoint.rotation, targetScale));
            }
            else
            {
                StartCoroutine(ScaleAndMoveObject(originalPosition, originalRotation, originalScale));
            }

            isScaled = !isScaled;
        }
    }

    private IEnumerator ScaleAndMoveObject(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
    {
        float time = 0;
        Vector3 startScale = rootObject.localScale;
        Vector3 startPosition = rootObject.position;
        Quaternion startRotation = rootObject.rotation;

        while (time < 1)
        {
            time += Time.deltaTime * scaleSpeed;
            rootObject.localScale = Vector3.Lerp(startScale, targetScale, time);
            rootObject.position = Vector3.Lerp(startPosition, targetPosition, time);
            rootObject.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            yield return null;
        }

        // Ensure final values are exactly as expected
        rootObject.localScale = targetScale;
        rootObject.position = targetPosition;
        rootObject.rotation = targetRotation;
    }
}
