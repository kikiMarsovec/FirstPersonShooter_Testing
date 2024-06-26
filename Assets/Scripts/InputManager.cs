using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// S to skripto manegamo inpute in klicemo ustrezne metode iz skript ki izvedejo metode

public class InputManager : MonoBehaviour {

    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;

    private PlayerMovement playerMovement;
    private MouseLook mouseLook;
    private Gun gun;

    void Awake() {
        playerInput = new PlayerInput();
        playerActions = playerInput.Player;

        // gun controlls
        gun = gameObject.transform.GetChild(0).GetComponentInChildren<Gun>(); // we get main camera and then search its children for the Gun script
        // move player
        playerMovement = GetComponent<PlayerMovement>();
        // look around 
        mouseLook = GetComponent<MouseLook>();
        // perform jump
        playerActions.Jump.performed += ctx => playerMovement.Jump();
        // start  and stop running
        playerActions.Run.started += ctx => playerMovement.StartRunning();
        playerActions.Run.canceled += ctx => playerMovement.EndRunning();
        // toggle crouch
        playerActions.Crouch.performed += ctx => playerMovement.ToggleCrouch();
        // fire gun
        playerActions.Shoot.performed += ctx => gun.Shoot();
    }

	private void Update() {
        mouseLook.ProcessLook(playerActions.Look.ReadValue<Vector2>());
		playerMovement.ProcessMove(playerActions.Movement.ReadValue<Vector2>());
	}

	private void OnEnable() {
		playerActions.Enable();
	}
    private void OnDisable() {
        playerActions.Disable();
    }
}
