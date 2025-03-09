using UnityEngine;
using Oculus.Interaction;

[System.Serializable]
public class ModelPart
{
    public GameObject part;
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

    [Header("Interaction")]
    [SerializeField]
    private PokeInteractable pokeInteractable;

    public bool isExploded = false;
    private bool previousExplodedState = false;

    private void Start()
    {
        StoreOriginalPositions();

        if (pokeInteractable != null)
            pokeInteractable.WhenStateChanged += HandlePoke;
    }

    private void OnDestroy()
    {
        if (pokeInteractable != null)
            pokeInteractable.WhenStateChanged -= HandlePoke;
    }

    private void Update()
    {
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
            if (mp.part != null)
                mp.originalPosition = mp.part.transform.localPosition;
        }
    }

    public void ExplodeView()
    {
        StopAllCoroutines();
        foreach (ModelPart mp in modelParts)
        {
            if (mp.part != null)
                StartCoroutine(MovePart(mp.part, mp.part.transform.localPosition, mp.explodedPosition));
        }
    }

    public void ImplodeView()
    {
        StopAllCoroutines();
        foreach (ModelPart mp in modelParts)
        {
            if (mp.part != null)
                StartCoroutine(MovePart(mp.part, mp.part.transform.localPosition, mp.originalPosition));
        }
    }

    private System.Collections.IEnumerator MovePart(GameObject part, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;
        while (elapsed < explosionDuration)
        {
            part.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / explosionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        part.transform.localPosition = endPos;
    }

    private void HandlePoke(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            isExploded = !isExploded;
        }
    }
}
