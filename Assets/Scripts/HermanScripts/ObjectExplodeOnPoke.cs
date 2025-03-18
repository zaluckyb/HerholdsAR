using UnityEngine;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;

public class ObjectExplodeOnPoke : MonoBehaviour
{
    [Tooltip("Root object where objects explode from")]
    [SerializeField]
    private Transform rootObject;

    [Tooltip("Objects to explode outward from the root object")]
    [SerializeField]
    private GameObject[] objectsToExplode;

    [Tooltip("Spacing factor between objects in the exploded view")]
    [SerializeField]
    private float spacingMultiplier = 1.5f;

    [Tooltip("Explosion speed (higher means faster spread)")]
    [SerializeField]
    private float explosionSpeed = 2f;

    [Tooltip("Reference to the PokeInteractable component")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    private Dictionary<GameObject, Vector3> originalLocalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> explodedLocalPositions = new Dictionary<GameObject, Vector3>();
    private bool isExploded = false;

    private void Start()
    {
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

        if (objectsToExplode.Length == 0)
        {
            Debug.LogError("No objects assigned to explode.", this);
            return;
        }

        foreach (var obj in objectsToExplode)
        {
            if (obj != null)
            {
                originalLocalPositions[obj] = rootObject.InverseTransformPoint(obj.transform.position);
            }
        }

        CalculateExplodedPositions();
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
            StartCoroutine(MoveObjects(isExploded ? originalLocalPositions : explodedLocalPositions));
            isExploded = !isExploded;
        }
    }

    private void CalculateExplodedPositions()
    {
        explodedLocalPositions.Clear();
        
        float spacing = spacingMultiplier;
        float totalWidth = (objectsToExplode.Length - 1) * spacing;
        float startX = -totalWidth * 0.5f;
        float currentX = startX;

        for (int i = 0; i < objectsToExplode.Length; i++)
        {
            GameObject obj = objectsToExplode[i];
            if (obj != null)
            {
                Vector3 newLocalPosition = new Vector3(currentX, originalLocalPositions[obj].y, originalLocalPositions[obj].z);
                explodedLocalPositions[obj] = newLocalPosition;
                currentX += spacing;
            }
        }
    }

    private IEnumerator MoveObjects(Dictionary<GameObject, Vector3> targetLocalPositions)
    {
        float time = 0;
        Dictionary<GameObject, Vector3> startLocalPositions = new Dictionary<GameObject, Vector3>();

        foreach (var obj in objectsToExplode)
        {
            if (obj != null)
            {
                startLocalPositions[obj] = rootObject.InverseTransformPoint(obj.transform.position);
            }
        }

        while (time < 1)
        {
            time += Time.deltaTime * explosionSpeed;
            foreach (var obj in objectsToExplode)
            {
                if (obj != null)
                {
                    Vector3 newLocalPosition = Vector3.Lerp(startLocalPositions[obj], targetLocalPositions[obj], time);
                    obj.transform.position = rootObject.TransformPoint(newLocalPosition);
                }
            }
            yield return null;
        }
    }
}