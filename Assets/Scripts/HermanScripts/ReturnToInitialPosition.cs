using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;

public class ReturnToInitialPosition : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    private HandGrabInteractable handGrabInteractable;
    private Rigidbody objectRigidbody;
    private int grabCount = 0;

    [SerializeField]
    private float returnDuration = 1.0f; // Duration for the return animation

    void Start()
    {
        // Find the HandGrabInteractable component in the children
        handGrabInteractable = GetComponentInChildren<HandGrabInteractable>();
        if (handGrabInteractable == null)
        {
            Debug.LogError("HandGrabInteractable component not found in children!");
            return;
        }

        // Find the Rigidbody component
        objectRigidbody = GetComponent<Rigidbody>();
        if (objectRigidbody == null)
        {
            Debug.LogError("Rigidbody component not found!");
            return;
        }

        // Store the initial position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Subscribe to the grab events
        handGrabInteractable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDestroy()
    {
        if (handGrabInteractable != null)
        {
            // Unsubscribe from the grab events
            handGrabInteractable.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            grabCount++;
            StopAllCoroutines(); // Stop any ongoing return animation
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            grabCount--;
            if (grabCount <= 0)
            {
                StartCoroutine(SmoothReturnToInitialPosition());
            }
        }
    }

    private IEnumerator SmoothReturnToInitialPosition()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0.0f;

        // Set the object to kinematic to avoid physics interactions
        objectRigidbody.isKinematic = true;

        while (elapsedTime < returnDuration)
        {
            transform.position = Vector3.Lerp(startPosition, initialPosition, elapsedTime / returnDuration);
            transform.rotation = Quaternion.Lerp(startRotation, initialRotation, elapsedTime / returnDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Reset kinematic state to its original value
        objectRigidbody.isKinematic = false;
    }
}
