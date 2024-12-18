using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
    [Header("Run Settings")] [SerializeField]
    XRNode leftHandNode = XRNode.LeftHand;

    [SerializeField] XRNode rightHandNode = XRNode.RightHand;
    [SerializeField] float movementSpeedRatio = 3f;
    [SerializeField] float swingThreshold = 0.1f; // Minimum movement to count as a swing
    [SerializeField] DynamicMoveProvider dynamicMoveProvider;

    private Vector3 leftHandPrevPosition;
    private Vector3 rightHandPrevPosition;
    private float leftSwingSpeed;
    private float rightSwingSpeed;
    private float originalMoveSpeed;

    [Header("Jump Settings")] [SerializeField]
    private InputActionReference jumpAction;

    [SerializeField] CharacterController characterController;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravityValue = -15f;
    private Vector3 playerVelocity;
    private float currentGravity;

    [Header("Far Interaction Settings")] 
    [SerializeField] float farInteractionDistance = 10f;
    [SerializeField] float sphereCastRadius = 0.25f;
    [SerializeField] NearFarInteractor rightHandInteractor;
    [SerializeField] NearFarInteractor leftHandInteractor;
    
    // CLIMB SETTINGS
    private static List<ClimbInteractable> climbInteractables = new List<ClimbInteractable>();
    private static bool isClimbing = false;

    void OnEnable()
    {
        jumpAction.action.Enable();
        jumpAction.action.started += Jump;
    }

    void OnDisable()
    {
        jumpAction.action.Disable();
        jumpAction.action.started -= Jump;
    }

    private void Start()
    {
        currentGravity = gravityValue;
        originalMoveSpeed = dynamicMoveProvider.moveSpeed;
        leftHandPrevPosition = InputTracking.GetLocalPosition(leftHandNode);
        rightHandPrevPosition = InputTracking.GetLocalPosition(rightHandNode);
    }

    void Update()
    {
        TryMove();
        TryRun();
        TryFarInteract(leftHandInteractor);
        TryFarInteract(rightHandInteractor);
    }

    private void TryFarInteract(NearFarInteractor controller)
    {
        if (Physics.SphereCast(controller.transform.position, sphereCastRadius, controller.transform.forward, out var hit,
                farInteractionDistance))
        {
            if (hit.collider.CompareTag("Book"))
            {
                controller.enableFarCasting = true;
            }
        }
    }

    private void TryMove()
    {
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (!isClimbing)
        {
            playerVelocity.y += currentGravity * Time.deltaTime;
        }

        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void TryRun()
    {
        // Get current hand positions
        Vector3 leftHandPosition = InputTracking.GetLocalPosition(leftHandNode);
        Vector3 rightHandPosition = InputTracking.GetLocalPosition(rightHandNode);

        // Calculate swing speeds
        leftSwingSpeed = (leftHandPosition - leftHandPrevPosition).magnitude / Time.deltaTime;
        rightSwingSpeed = (rightHandPosition - rightHandPrevPosition).magnitude / Time.deltaTime;

        // Check for alternating motion
        if (!isClimbing && leftSwingSpeed > swingThreshold && rightSwingSpeed > swingThreshold &&
            Vector3.Dot(leftHandPosition - leftHandPrevPosition, rightHandPosition - rightHandPrevPosition) < 0)
        {
            // Move the player forward
            dynamicMoveProvider.moveSpeed = Mathf.Max(leftSwingSpeed, rightSwingSpeed) * movementSpeedRatio;
        }
        else
        {
            dynamicMoveProvider.moveSpeed = isClimbing ? 0 : Mathf.Lerp(dynamicMoveProvider.moveSpeed, originalMoveSpeed, Time.deltaTime);
        }

        // Update previous positions
        leftHandPrevPosition = leftHandPosition;
        rightHandPrevPosition = rightHandPosition;
    }


    private void Jump(InputAction.CallbackContext obj)
    {
        if (!characterController.isGrounded || isClimbing) return;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    public static void OnClimbSelected(SelectEnterEventArgs args)
    {
        ClimbInteractable climbInteractable = args.interactableObject as ClimbInteractable;
        if (climbInteractable == null) return;
        climbInteractables.Add(climbInteractable);
        isClimbing = true;
    }

    public static void OnClimbDeselected(SelectExitEventArgs args)
    {
        ClimbInteractable climbInteractable = args.interactableObject as ClimbInteractable;
        if (climbInteractable == null) return;
        climbInteractables.Remove(climbInteractable);
        isClimbing = climbInteractables.Count > 0;
    }
    public void SetPlayerHandInteractors(bool state)
    {
        leftHandInteractor.enabled = state;
        rightHandInteractor.enabled = state;
    }
}