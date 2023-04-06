using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Input input;
    [HideInInspector] public Vector2 directionInput = Vector2.zero;
    [HideInInspector] public 

    void Awake()
    {
        input = new Input();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Direction.performed += OnMovementPerformed;
        input.Player.Direction.canceled += OnMovementCancelled;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Direction.performed -= OnMovementPerformed;
        input.Player.Direction.canceled -= OnMovementCancelled;
    }

    private void OnMovementPerformed(InputAction.CallbackContext val)
    {
        directionInput = val.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext val)
    {
        directionInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        //Debug.Log(directionInput);
    }
    
}
