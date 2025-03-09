using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

public class PokeMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class PokeInteractableMenuPair
    {
        public PokeInteractable pokeInteractable;  // Assign your interactable button here
        public GameObject menu;                    // Corresponding menu to activate
    }

    [Header("Poke Interactable - Menu Pairs")]
    public List<PokeInteractableMenuPair> pokeMenuPairs;

    private void Start()
    {
        ResetAllMenus();

        foreach (var pair in pokeMenuPairs)
        {
            var localPair = pair; // Avoid closure issues
            localPair.pokeInteractable.WhenStateChanged += args => HandlePoke(localPair, args);
        }
    }

    private void OnDestroy()
    {
        foreach (var pair in pokeMenuPairs)
        {
            if (pair.pokeInteractable != null)
                pair.pokeInteractable.WhenStateChanged -= args => HandlePoke(pair, args);
        }
    }

    private void HandlePoke(PokeInteractableMenuPair pair, InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            ActivateMenu(pair);
        }
    }

    private void ActivateMenu(PokeInteractableMenuPair activePair)
    {
        foreach (var pair in pokeMenuPairs)
        {
            bool isActive = pair == activePair;

            if (pair.menu != null)
                pair.menu.SetActive(isActive);

            pair.pokeInteractable.enabled = !isActive;
        }
    }

    public void ResetAllMenus()
    {
        foreach (var pair in pokeMenuPairs)
        {
            if (pair.menu != null)
                pair.menu.SetActive(false);

            pair.pokeInteractable.enabled = true;
        }
    }

    public void OnResetButtonClicked()
    {
        ResetAllMenus();
    }
}
