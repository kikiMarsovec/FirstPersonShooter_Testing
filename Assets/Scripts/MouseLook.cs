using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    // sensitivity za hitrost rotacije
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    public float smoothLook = 0.03f;

	public Transform camera;
	Vector2 currentInputVector;
	Vector2 smoothInputVelocity;

	float xRotation = 0f;

    void Start() {
        // zaklenemo misko in jo skrijemo
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ProcessLook(Vector2 input) {
		// mouse look smoothing
		currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothLook);

		// dobimo input od miske
		float mouseX = currentInputVector.x * xSensitivity * Time.deltaTime;
        float mouseY = currentInputVector.y * ySensitivity * Time.deltaTime;

        // GorDol rotacija (moramo clampati da ne pogledamo v nazaj)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotiramo kamero z pridobljeno GorDol rotacijo
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // rotiramo playerja z LevoDesno rotacijo
        transform.Rotate(Vector3.up * mouseX);
    }
}
