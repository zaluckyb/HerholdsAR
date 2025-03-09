using UnityEngine;
using System.Collections;

[System.Serializable]
public class ModelPart
{
    public Transform part;
    [HideInInspector] public Vector3 originalPosition;
    public Vector3 explodedPosition;
}

public class ExplodedViewManager : MonoBehaviour
{
    [Header("Model References")]
    public GameObject modelRoot;
    public ModelPart[] modelParts;

    [Header("Exploded View Settings")]
    public float explosionDuration = 0.5f;
    public bool isExploded = false;

    // Private to track changes
    private bool previousExplodedState = false;

    private void Start()
    {
        StoreOriginalPositions();
        previousExplodedState = isExploded;
    }

    private void Update()
    {
        // Detect manual change of "isExploded" in Inspector at runtime
        if (previousExplodedState != isExploded)
        {
            previousExplodedState = isExploded;
            if (isExploded)
                ExplodeView();
            else
                ImplodeView();
        }
    }

    public void StoreOriginalPositions()
    {
        foreach (ModelPart mp in modelParts)
        {
            mp.originalPosition = mp.part.localPosition;
        }
    }

    public void ExplodeView()
    {
        StopAllCoroutines();
        foreach (ModelPart mp in modelParts)
        {
            StartCoroutine(MovePart(mp.part, mp.part.localPosition, mp.explodedPosition));
        }
    }

    public void ImplodeView()
    {
        StopAllCoroutines();
        foreach (ModelPart mp in modelParts)
        {
            StartCoroutine(MovePart(mp.part, mp.part.localPosition, mp.originalPosition));
        }
    }

    private IEnumerator MovePart(Transform part, Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;
        while (elapsedTime < explosionDuration)
        {
            part.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / explosionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        part.localPosition = endPos;
    }
}
