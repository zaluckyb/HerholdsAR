using UnityEngine;
using Oculus.Interaction;
using System.Collections;

public class ItemShowCase : MonoBehaviour
{
    [Tooltip("Object to move and scale when poked")]
    [SerializeField]
    private GameObject targetObject;

    [Tooltip("Target transform defining position and scale")]
    [SerializeField]
    private Transform targetTransform;

    [Tooltip("Movement speed of the object")]
    [SerializeField]
    private float moveSpeed = 2f;

    [Tooltip("Scaling speed of the object")]
    [SerializeField]
    private float scaleSpeed = 2f;

    [Tooltip("Reference to the PokeInteractable component")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private bool isMoved = false;

    private void Start()
    {
        if (pokeInteractable == null)
        {
            Debug.LogError("PokeInteractable component not assigned.", this);
            return;
        }
        if (targetObject == null || targetTransform == null)
        {
            Debug.LogError("Target Object or Target Transform not assigned.", this);
            return;
        }

        // Store initial position and scale
        initialPosition = targetObject.transform.position;
        initialScale = targetObject.transform.localScale;

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
            StopAllCoroutines();
            if (isMoved)
            {
                StartCoroutine(MoveAndScale(targetObject, initialPosition, initialScale));
            }
            else
            {
                StartCoroutine(MoveAndScale(targetObject, targetTransform.position, targetTransform.localScale));
            }
            isMoved = !isMoved;
        }
    }

    private IEnumerator MoveAndScale(GameObject obj, Vector3 targetPos, Vector3 targetScale)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 startScale = obj.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            obj.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
            obj.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime * scaleSpeed);
            yield return null;
        }

        obj.transform.position = targetPos;
        obj.transform.localScale = targetScale;
    }
}