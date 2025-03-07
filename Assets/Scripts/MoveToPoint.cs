using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    [Header("Drag the target GameObject (destination)")]
    public GameObject targetObject;

    [Header("Drag the GameObject to move")]
    public GameObject movingObject;

    [Header("Movement Settings")]
    [Tooltip("Duration in seconds for the movement")]
    public float moveDuration = 1f;

    [Header("Scale Settings")]
    [Tooltip("Target scale for the moving object")]
    public Vector3 targetScale = Vector3.one;

    private bool shouldMove = false;
    private float elapsedTime = 0f;
    private Vector3 startPosition;
    private Vector3 startScale;
    private Vector3 endPosition;

    private void OnEnable()
    {
        if (targetObject == null || movingObject == null)
        {
            Debug.LogWarning("Please assign both the target object and the moving object in the inspector.");
            return;
        }

        startPosition = movingObject.transform.position;
        startScale = movingObject.transform.localScale;
        endPosition = targetObject.transform.position;
        elapsedTime = 0f;
        shouldMove = true;
    }

    private void Update()
    {
        if (shouldMove)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            movingObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            movingObject.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (t >= 1f)
            {
                shouldMove = false;
            }
        }
    }
}