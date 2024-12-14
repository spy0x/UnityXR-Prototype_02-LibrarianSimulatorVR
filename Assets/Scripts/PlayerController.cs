using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] CharacterController characterController;
    [SerializeField] float jumpHeight = 1.0f;
    private Vector3 playerVelocity;
    private float gravityValue = -9.81f;
    
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
    void Update()
    {
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (characterController.isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
    }
}
