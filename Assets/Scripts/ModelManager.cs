using UnityEngine;

[System.Serializable]
public class ManagedModel
{
    public string modelName;
    public GameObject modelObject;

    public Vector3 initialPosition;
    public Vector3 initialEulerRotation;
    public Vector3 initialScale = Vector3.one;

    public bool resetTransform = false;

    public void SetInitialTransform()
    {
        if (modelObject != null)
        {
            modelObject.transform.position = initialPosition;
            modelObject.transform.rotation = Quaternion.Euler(initialEulerRotation);
            modelObject.transform.localScale = initialScale;
        }
    }

    public void CaptureCurrentTransform()
    {
        if (modelObject != null)
        {
            initialPosition = modelObject.transform.position;
            initialEulerRotation = modelObject.transform.rotation.eulerAngles;
            initialScale = modelObject.transform.localScale;
        }
    }
}

public class ModelManager : MonoBehaviour
{
    [Header("Managed Models")]
    public ManagedModel[] managedModels;

    void Start()
    {
        foreach (var model in managedModels)
        {
            model.SetInitialTransform();
        }
    }

    void OnValidate()
    {
        foreach (var model in managedModels)
        {
            if (model.modelObject != null && model.initialScale == Vector3.zero)
                model.CaptureCurrentTransform();
        }
    }

    void Update()
    {
        foreach (var model in managedModels)
        {
            if (model.resetTransform)
            {
                model.SetInitialTransform();
                model.resetTransform = false;
            }
        }
    }
}