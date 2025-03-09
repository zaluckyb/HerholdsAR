using UnityEngine;
using Oculus.Interaction;

[System.Serializable]
public class ModelPart
{
    public GameObject part;

    [HideInInspector] public Vector3 originalPosition;
    public Vector3 explodedPosition;

    [HideInInspector] public Quaternion originalRotation;
    public Vector3 explodedEulerRotation;

    [HideInInspector] public Vector3 originalScale;
    public Vector3 explodedScale = Vector3.one;

    [HideInInspector] public GameObject previousPart;

    public void UpdateOriginalValues()
    {
        if (part != previousPart && part != null)
        {
            originalPosition = part.transform.localPosition;
            explodedPosition = originalPosition;

            originalRotation = part.transform.localRotation;
            explodedEulerRotation = originalRotation.eulerAngles;

            originalScale = part.transform.localScale;
            explodedScale = originalScale;

            previousPart = part;
        }
    }
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
        StoreOriginalStates();

        if (pokeInteractable != null)
            pokeInteractable.WhenStateChanged += HandlePoke;
    }

    private void OnDestroy()
    {
        if (pokeInteractable != null)
            pokeInteractable.WhenStateChanged -= HandlePoke;
    }

    private void OnValidate()
    {
        foreach (ModelPart mp in modelParts)
        {
            mp.UpdateOriginalValues();
        }
    }

    public void StoreOriginalStates()
    {
        foreach (ModelPart mp in modelParts)
        {
            if (mp.part != null)
            {
                mp.originalPosition = mp.part.transform.localPosition;
                mp.originalScale = mp.part.transform.localScale;
                mp.originalRotation = mp.part.transform.localRotation;
            }
        }
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

    public void ExplodeView()
    {
        StopAllCoroutines();
        foreach (ModelPart mp in modelParts)
        {
            if (mp.part != null)
                StartCoroutine(MoveRotateScalePart(mp.part, mp.part.transform.localPosition, mp.explodedPosition, mp.part.transform.localRotation, Quaternion.Euler(mp.explodedEulerRotation), mp.part.transform.localScale, mp.explodedScale));
        }
    }

    public void ImplodeView()
    {
        StopAllCoroutines();
        foreach (ModelPart mp in modelParts)
        {
            if (mp.part != null)
                StartCoroutine(MoveRotateScalePart(mp.part, mp.part.transform.localPosition, mp.originalPosition, mp.part.transform.localRotation, mp.originalRotation, mp.part.transform.localScale, mp.originalScale));
        }
    }

    private System.Collections.IEnumerator MoveRotateScalePart(GameObject part, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, Vector3 startScale, Vector3 endScale)
    {
        float elapsed = 0f;
        while (elapsed < explosionDuration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / explosionDuration);

            part.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            part.transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            part.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        part.transform.localPosition = endPos;
        part.transform.localRotation = endRot;
        part.transform.localScale = endScale;
    }

    private void HandlePoke(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            isExploded = !isExploded;
        }
    }
}
