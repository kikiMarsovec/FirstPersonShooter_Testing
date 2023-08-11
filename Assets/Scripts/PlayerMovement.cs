using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour {

    private CharacterController controller;

	public float walkingSpeed = 4f;
	public float runningSpeed = 10f;
    private float speed;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
	public float smoothMovement = 0.12f;
	public float smoothStartRunning = 0.2f;
	public float smoothEndRunning = 0.2f;
	public float smoothCrouchingSpeed = 0.2f;

	public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
	Vector2 currentInputVector;
	Vector2 smoothInputVelocity;
	bool startedRunning = false;
	bool endedRunning = false;
	bool shiftDown = false;
	float currentSpeed;
	float currentVelocity;
	bool crouching = false;
	bool crouchSmoothing = false;
	Vector3 currentHeight = Vector3.one;
	Vector3 standingHeight = Vector3.one;
	Vector3 crouchingHeight = new Vector3(1f, 0.5f, 1f);
	Vector3 crouchingVelocity;

	void Start() {
        controller = GetComponent<CharacterController>();
		speed = walkingSpeed;
		currentSpeed = walkingSpeed;
    }

	public void ProcessMove(Vector2 input) {
		// preverimo ali je player na tleh, da iznicimo gravitacijo
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		if (isGrounded && velocity.y < 0)
			velocity.y = -2f;

		// TODO CROUCHING:
		// zmanjsa se walkingSpeed in RunningSpeed (ali pa sploh nimamo vec opcije RunningSpeed, odloci se kako zelis)
		// ne moremo skakati (oz ce skocimo ubistvu uncrouchamo)
		// ce uncrouchamo in se zadanemo v nek objekt (nad nami) potem gremo avtomatsko nazaj v crouch
		// Pri uncrouchanju dobimo nekaksen "Jitter", ne vem kako razresit 

		//  crouch  smoothing
		if (crouchSmoothing) {
			if (crouching) {  // crouching
				currentHeight = Vector3.SmoothDamp(currentHeight, crouchingHeight, ref crouchingVelocity, smoothCrouchingSpeed);
				if (currentHeight == crouchingHeight) {
					crouchSmoothing = false;
				}
			} else { // standing up
				currentHeight = Vector3.SmoothDamp(currentHeight, standingHeight, ref crouchingVelocity, smoothCrouchingSpeed);
				if (currentHeight == standingHeight) {
					crouchSmoothing = false;
				}
			}
			transform.localScale = currentHeight;
		}

		// walk-run transitions smoothing
		if (shiftDown) {
			if (input == Vector2.zero ) {
				startedRunning = false;
				endedRunning = true;
			} else {
				startedRunning = true;
				endedRunning = false;
			}
		}
		if (startedRunning) {
			if (currentSpeed < walkingSpeed)
				currentSpeed = walkingSpeed;
			currentSpeed = Mathf.SmoothDamp(currentSpeed, runningSpeed, ref currentVelocity, smoothStartRunning);
			speed = currentSpeed;
			if (currentSpeed >= runningSpeed - 0.01f)
				startedRunning = false;
		}
		if (endedRunning) {
			if (currentSpeed > runningSpeed)
				currentSpeed = runningSpeed;
			currentSpeed = Mathf.SmoothDamp(currentSpeed, walkingSpeed, ref currentVelocity, smoothEndRunning);
			speed = currentSpeed;
			if (currentSpeed <= walkingSpeed + 0.01f)
				endedRunning = false;
		}

		// smoothan zacetek-konec premikanja playerja
		currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothMovement);
		Vector3 move = transform.right * currentInputVector.x + transform.forward * currentInputVector.y;
		controller.Move(move * speed * Time.deltaTime);

		// gravitacija
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

    public void Jump() {
        // preverimo ali je player na tleh
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		if (isGrounded) {
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // skocimo
		}

		// TODO pri skakanju se moramo, ce se zabijemo v strop, odbiti nazaj dol 
	}

	public void StartRunning() {
		shiftDown = true;
	}

	public void EndRunning() {
		shiftDown = false;
		startedRunning = false;
		endedRunning = true;
	}

	public void ToggleCrouch() {
		crouching = !crouching;
		crouchSmoothing = true;
		/*
		if (crouching) {
			transform.localScale = new Vector3(1f, 0.5f, 1f);
		} else {
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		*/
	}
}
