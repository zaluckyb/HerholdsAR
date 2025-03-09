using UnityEngine;
using Oculus.Interaction;
using System.Collections;

[System.Serializable]
public class ModelPart
{
    public GameObject part;

    [HideInInspector] public Vector3 originalPosition;
    public Vector3 explodedPosition;

    [HideInInspector] public Quaternion originalRotation;
    public Vector3 explodedEulerRotation;

    [HideInInspector] public Vector3 originalScale;
    public Vector3 explodedScale;

    private GameObject previousPart;

    public void UpdateOriginalStates()
    {
        if (part != null && previousPart != part)
        {
            originalPosition = part.transform.localPosition;
            originalRotation = part.transform.localRotation;
            originalScale = part.transform.localScale;

            explodedScale = explodedScale == Vector3.zero ? originalScale : explodedScale;
            previousPart = part;
        }
    }
}

[System.Serializable]
public class Model
{
    public string modelName;
    public GameObject modelRoot;
    public ModelPart[] modelParts;
    public PokeInteractable pokeInteractable;
    public float explosionDuration = 1.0f;

    [HideInInspector] public bool isExploded = false;

    public void UpdateAllOriginalStates()
    {
        foreach (var part in modelParts)
            part.UpdateOriginalStates();
    }
}

public class ExplodedViewManager : MonoBehaviour
{
    [Header("Models Array")]
    public Model[] models;

    private void Start()
    {
        foreach (var model in models)
        {
            model.UpdateAllOriginalStates();

            if (model.pokeInteractable != null)
                model.pokeInteractable.WhenStateChanged += (args) => HandlePoke(args, model);
        }
    }

    private void OnDestroy()
    {
        foreach (var model in models)
        {
            if (model.pokeInteractable != null)
                model.pokeInteractable.WhenStateChanged -= (args) => HandlePoke(args, model);
        }
    }

    private void HandlePoke(InteractableStateChangeArgs args, Model model)
    {
        if (args.NewState == InteractableState.Select)
        {
            StopAllCoroutines();
            model.isExploded = !model.isExploded;
            StartCoroutine(ExplodeModel(model, model.isExploded));
        }
    }

    private IEnumerator MovePart(Transform part, Vector3 targetPos, Quaternion targetRot, Vector3 targetScale, float duration)
    {
        Vector3 startPos = part.localPosition;
        Quaternion startRot = part.localRotation;
        Vector3 startScale = part.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            part.localPosition = Vector3.Lerp(startPos, targetPos, t);
            part.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            part.localScale = Vector3.Lerp(startScale, targetScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        part.localPosition = targetPos;
        part.localRotation = targetRot;
        part.localScale = targetScale;
    }

    private IEnumerator ExplodeModel(Model model, bool explode)
    {
        foreach (var mp in model.modelParts)
        {
            Vector3 targetPos = explode ? mp.explodedPosition : mp.originalPosition;
            Quaternion targetRot = explode ? Quaternion.Euler(mp.explodedEulerRotation) : mp.originalRotation;
            Vector3 targetScale = explode ? mp.explodedScale : mp.originalScale;

            StartCoroutine(MovePart(mp.part.transform, targetPos, targetRot, targetScale, model.explosionDuration));
        }

        yield return new WaitForSeconds(model.explosionDuration);
    }

    public void ExplodeAll()
    {
        foreach (var model in models)
        {
            StopAllCoroutines();
            model.isExploded = true;
            StartCoroutine(ExplodeModel(model, true));
        }
    }

    public void ImplodeAll()
    {
        foreach (var model in models)
        {
            StopAllCoroutines();
            model.isExploded = false;
            StartCoroutine(ExplodeModel(model, false));
        }
    }
}