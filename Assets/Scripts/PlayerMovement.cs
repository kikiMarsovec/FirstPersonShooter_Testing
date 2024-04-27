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
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

	public Transform headCheck;
	public float headDistance = 0.1f;

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
	float currentHeight = 1.7f;
	float standingHeight = 1.7f;
	float crouchingHeight = 1.01f;
	float crouchingVelocity;

	bool jumpHit = false;

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

		// preverimo ce skace in se ravno zabije v strop z glavo
		if (!isGrounded && !jumpHit && Physics.CheckSphere(headCheck.position, headDistance, groundMask)) {
			velocity.y = -1 * (velocity.y * 0.2f);
			jumpHit = true;
		} else if (isGrounded) {
			jumpHit = false;
		}

		//  crouch  smoothing
		if (crouchSmoothing) {
			Debug.Log(Time.time);
			if (crouching) {  // crouching
				currentHeight = Mathf.SmoothDamp(currentHeight, crouchingHeight, ref crouchingVelocity, smoothCrouchingSpeed);
				if (currentHeight <= crouchingHeight + 0.01f) {
					currentHeight = crouchingHeight;
					crouchSmoothing = false;
				}
			} else { // standing up
				// preverimo ce  se  v strop zaletimo z glavo  (ce se, gremo nazaj v crouch)
				if (Physics.CheckSphere(headCheck.position, headDistance, groundMask)) {
					Crouch();
				}

				// razresimo jitter ko ustajamo
				if (input.y == 0)
					input.y = 0.1f;

				currentHeight = Mathf.SmoothDamp(currentHeight, standingHeight, ref crouchingVelocity, smoothCrouchingSpeed);
				if (currentHeight >= standingHeight - 0.01f) {
					currentHeight = standingHeight;
					crouchSmoothing = false;
				}
			}
			//transform.localScale = currentHeight;
			controller.height = currentHeight;
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
			if (crouching)
				StandUp(); // ce smo crouchani, vstanemo
			else
				velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // skocimo
		}
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
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		if (!isGrounded)
			return;
		if (crouching)
			StandUp();
		else
			Crouch();
	}

	private void Crouch() {
		crouching = true;
		crouchSmoothing = true;
		walkingSpeed /= 2;
		runningSpeed/= 2;
		speed = walkingSpeed;
	}

	private void StandUp() {
		crouching = false;
		crouchSmoothing = true;
		walkingSpeed *= 2;
		runningSpeed *= 2;
		speed = walkingSpeed;
	}
}
