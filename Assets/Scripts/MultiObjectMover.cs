using UnityEngine;
using Oculus.Interaction;
using System.Collections;

[System.Serializable]
public class MoveableObject
{
    public string objectName;

    [Header("Objects")]
    public GameObject targetObject;
    public GameObject movingObject;

    [Header("Interaction")]
    public PokeInteractable pokeInteractable;

    [Header("Settings")]
    public float moveDuration = 1f;
    public Vector3 targetScale = Vector3.one;

    [Header("Objects to Toggle")]
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Vector3 originalScale;

    [HideInInspector] public bool isMoved = false;

    public void StoreOriginalTransforms()
    {
        if (movingObject != null)
        {
            originalPosition = movingObject.transform.position;
            originalScale = movingObject.transform.localScale;
        }
    }

    public void ToggleObjects(bool activate)
    {
        foreach (var obj in objectsToActivate)
            obj.SetActive(activate);

        foreach (var obj in objectsToDeactivate)
            obj.SetActive(!activate);
    }
}

public class MultiObjectMover : MonoBehaviour
{
    public MoveableObject[] moveableObjects;

    void Start()
    {
        foreach (var obj in moveableObjects)
        {
            obj.StoreOriginalTransforms();

            if (obj.pokeInteractable != null)
                obj.pokeInteractable.WhenStateChanged += (args) => HandlePoke(args, obj);
        }
    }

    void OnDestroy()
    {
        foreach (var obj in moveableObjects)
        {
            if (obj.pokeInteractable != null)
                obj.pokeInteractable.WhenStateChanged -= (args) => HandlePoke(args, obj);
        }
    }

    void HandlePoke(InteractableStateChangeArgs args, MoveableObject obj)
    {
        if (args.NewState == InteractableState.Select)
        {
            StopAllCoroutines();
            obj.isMoved = !obj.isMoved;

            if (obj.isMoved)
            {
                obj.ToggleObjects(true);
                StartCoroutine(MoveAndScale(obj, obj.movingObject.transform.position, obj.targetObject.transform.position, obj.movingObject.transform.localScale, obj.targetScale, obj.moveDuration));
            }
            else
            {
                obj.ToggleObjects(false);
                StartCoroutine(MoveAndScale(obj, obj.movingObject.transform.position, obj.originalPosition, obj.movingObject.transform.localScale, obj.originalScale, obj.moveDuration));
            }
        }
    }

    IEnumerator MoveAndScale(MoveableObject obj, Vector3 startPos, Vector3 endPos, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            obj.movingObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            obj.movingObject.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.movingObject.transform.position = endPos;
        obj.movingObject.transform.localScale = endScale;
    }
}