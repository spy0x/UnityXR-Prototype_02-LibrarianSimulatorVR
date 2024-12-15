using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class Bookshelf : MonoBehaviour
{
    [SerializeField] ClimbInteractable climbInteractable;
    [SerializeField] Transform teleportAnchor;

    // private void OnEnable()
    // {
    //     climbInteractable.selectEntered.AddListener(OnSelectEntered);
    // }
    //
    // private void OnDisable()
    // {
    //     climbInteractable.selectEntered.RemoveListener(OnSelectEntered);
    // }

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        Vector3 playerRelative = arg0.interactorObject.transform.GetComponentInParent<CharacterController>().transform.InverseTransformPoint(teleportAnchor.position);
        Vector3 teleportAnchorPos = teleportAnchor.localPosition;
        teleportAnchorPos.z -= playerRelative.z * 2;
        teleportAnchor.localPosition = teleportAnchorPos;
        Debug.Log(playerRelative);
        Debug.Log(teleportAnchorPos);
    }
}
