using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunCameraTilt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Raycaster groundCheck;

    [Header("Tilt")]
    [SerializeField, Range(0, 45)] private float cameraTilt = 20f;
    [SerializeField, Range(0, 90)] private float minAngle = 60f;
    [SerializeField] private float tiltSmoothing = 0.1f;

    [Header("Wall run")]
    [SerializeField] private Raycaster[] wallRunChecks;

    private CharacterController controller;
    private Vector3 currentRotationVelocity;

    private void Awake()
    {
        this.controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        var targetTilt = this.GetTargetTilt();
        var currentRotation = this.cameraTransform.rotation.eulerAngles;

        while (targetTilt - currentRotation.z > 180)
            targetTilt -= 360f;
        while (currentRotation.z - targetTilt > 180)
            targetTilt += 360f;

        var targetRotation = new Vector3(currentRotation.x, currentRotation.y, targetTilt);
        var actualRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref this.currentRotationVelocity, this.tiltSmoothing);

        this.cameraTransform.rotation = Quaternion.Euler(actualRotation);
    }

    private float GetTargetTilt()
    {
        if (this.controller.isGrounded || this.groundCheck.HasHit)
            return this.GetZero();

        var wallNormal = this.wallRunChecks.AggregateNormals();
        if (wallNormal == Vector3.zero)
            return this.GetZero();

        var dot = Vector3.Dot(transform.right, wallNormal);
        var angleThreshold = this.minAngle / 90;
        if (dot > angleThreshold)
            return this.cameraTilt * -1;
        else if (dot < -angleThreshold)
            return this.cameraTilt;
        else
            return this.GetZero();
    }

    private float GetZero()
    {
        var z = this.cameraTransform.rotation.eulerAngles.z;
        if (z > 180)
            return 360f;
        else
            return 0f;
    }
}
