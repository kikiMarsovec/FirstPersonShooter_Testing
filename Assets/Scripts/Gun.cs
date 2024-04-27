using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float damage = 10f;
    public float range = 100f;

    public Camera mainCamera;

    public ParticleSystem muzzleFlash;

    public void Shoot() {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);
        }
    }
}
