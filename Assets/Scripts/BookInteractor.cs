using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BookInteractor : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private BooksSpawner bookSpawner;
    [SerializeField] private InteractionLayerMask deactivatedLayerMask;
    public BooksSpawner BookSpawner => bookSpawner;
    public XRSocketInteractor SocketInteractor => socketInteractor;

    private void OnEnable()
    {
        socketInteractor.selectExited.AddListener(OnSelectExited);
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
    }


    private void OnDisable()
    {
        socketInteractor.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        bookSpawner.SpawnedBook.SetActive(false);
        XRGrabInteractable grabInteractable = arg0.interactableObject as XRGrabInteractable;
        if (grabInteractable)
        {
            grabInteractable.transform.tag = "Untagged";
            XRInteractableAffordanceStateProvider affordanceStateProvider =
                grabInteractable.GetComponent<XRInteractableAffordanceStateProvider>();
            if (affordanceStateProvider) affordanceStateProvider.enabled = false;
        }
    }

    private void OnSelectExited(SelectExitEventArgs arg0)
    {
        XRGrabInteractable grabInteractable = arg0.interactableObject as XRGrabInteractable;
        if (grabInteractable) grabInteractable.interactionLayers = deactivatedLayerMask;
    }
}