using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float pitch = 3f;
    public float zoomSpeed = 0.3f;
    public float minZoom = 3;
    public float maxZoom = 12;
    public float yawSpeed = 5;

    private float currentZoom = 10f;
    private float currentYaw = 0f;

    void Update()
    {
        currentZoom -= Input.GetAxis("Vertical") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (Input.GetButton("RotCam")) currentYaw -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
    }

    void LateUpdate()
    {
        transform.position = target.position - (offset * currentZoom);
        transform.LookAt(target.position + Vector3.up * pitch);
        transform.RotateAround(target.position, Vector3.up, currentYaw);
    }

    public void SetTarget(Transform value)
    {
        target = value;
    }
}
