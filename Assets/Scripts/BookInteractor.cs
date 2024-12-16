using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BookInteractor : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private BooksSpawner bookSpawner;
    public BooksSpawner BookSpawner => bookSpawner;
    public XRSocketInteractor SocketInteractor => socketInteractor;

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
        if (grabInteractable) grabInteractable.enabled = false;
        arg0.interactableObject.transform.tag = "Untagged";
    }

    public GameObject GetChildBook()
    {
        if (transform.childCount == 0) return null;
        return transform.GetChild(0).gameObject;
    }
}
