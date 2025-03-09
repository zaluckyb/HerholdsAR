using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.UnityCanvas;
using UnityEngine.Events;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public class InteractableMenuPair
    {
        public PokeInteractable interactable;
        public GameObject menu;
    }

    [Header("Interactable-Menu Pairs")]
    public List<InteractableMenuPair> interactableMenuPairs;

    void Start()
    {
        ResetAllMenus();

        // Assign event listeners dynamically
        foreach (var pair in interactableMenuPairs)
        {
            var wrapper = pair.interactable.GetComponent<InteractableUnityEventWrapper>();
            if (wrapper == null)
            {
                wrapper = pair.interactable.gameObject.AddComponent<InteractableUnityEventWrapper>();
            }

            wrapper.WhenSelect.AddListener(() => ActivateMenu(pair));
        }
    }

    void ActivateMenu(InteractableMenuPair activePair)
    {
        foreach (var pair in interactableMenuPairs)
        {
            bool isActive = pair == activePair;

            // Enable/Disable Interactable component
            pair.interactable.enabled = !isActive;

            // Enable/Disable corresponding menu
            if (pair.menu != null)
                pair.menu.SetActive(isActive);
        }
    }

    public void ResetAllMenus()
    {
        foreach (var pair in interactableMenuPairs)
        {
            if (pair.menu != null)
                pair.menu.SetActive(false);

            pair.interactable.enabled = true;
        }
    }

    // For resetting via a specific interactable
    public void OnResetButtonClicked()
    {
        ResetAllMenus();
    }
}
