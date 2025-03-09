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

            Vector3 startPos = obj.movingObject.transform.position;
            Vector3 endPos = obj.isMoved ? obj.targetObject.transform.position : obj.originalPosition;

            Vector3 startScale = obj.movingObject.transform.localScale;
            Vector3 endScale = obj.isMoved ? obj.targetScale : obj.originalScale;

            StartCoroutine(MoveAndScale(obj.movingObject, startPos, endPos, startScale, endScale, obj.moveDuration));
        }
    }

    IEnumerator MoveAndScale(GameObject movingObj, Vector3 startPos, Vector3 endPos, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            movingObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            movingObj.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        movingObj.transform.position = endPos;
        movingObj.transform.localScale = endScale;
    }
}
