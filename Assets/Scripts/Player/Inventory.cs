using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[System.Serializable]
public struct BodySocket
{
    public GameObject gameObject;
    [Range(0.01f, 1f)]
    public float heightRatio;
}

public class Inventory : MonoBehaviour

{
    [SerializeField] GameObject HMD;
    [SerializeField] BodySocket[] bodySockets;
    [SerializeField] private XRSocketInteractor rightSocketInteractor;
    [SerializeField] private XRSocketInteractor leftSocketInteractor;

    private Vector3 _currentHMDlocalPosition;
    private Quaternion _currentHMDRotation;

    private void OnEnable()
    {
        rightSocketInteractor.selectEntered.AddListener(OnSelectEntered);
        leftSocketInteractor.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        rightSocketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        leftSocketInteractor.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }
        
    }

    void Update()
    {
        _currentHMDlocalPosition = HMD.transform.localPosition;
        _currentHMDRotation = HMD.transform.rotation;
        foreach(var bodySocket in bodySockets)
        {
            UpdateBodySocketHeight(bodySocket);
        }
        UpdateSocketInventory();
    }

    private void UpdateBodySocketHeight(BodySocket bodySocket)
    {

        bodySocket.gameObject.transform.localPosition = new Vector3(bodySocket.gameObject.transform.localPosition.x,(_currentHMDlocalPosition.y * bodySocket.heightRatio), bodySocket.gameObject.transform.localPosition.z);
    }

    private void UpdateSocketInventory()
    {
        transform.localPosition = new Vector3(_currentHMDlocalPosition.x, 0, _currentHMDlocalPosition.z);
        transform.rotation = new Quaternion(transform.rotation.x, _currentHMDRotation.y, transform.rotation.z, _currentHMDRotation.w);
    }
}