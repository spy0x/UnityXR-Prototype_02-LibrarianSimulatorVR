using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class Bookshelf : MonoBehaviour
{
    [SerializeField] XRSocketInteractor socketInteractor;

    private void OnEnable()
    {
        socketInteractor.selectExited.AddListener(OnSelectExited);
    }
    
    private void OnDisable()
    {
        socketInteractor.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs arg0)
    {
        XRGrabInteractable grabInteractable = arg0.interactableObject as XRGrabInteractable;
        if (!grabInteractable) return;
        grabInteractable.enabled = false;
        XRInteractableAffordanceStateProvider affordanceStateProvider = grabInteractable.GetComponent<XRInteractableAffordanceStateProvider>();
        if (affordanceStateProvider) affordanceStateProvider.enabled = false;
    }
    
}
