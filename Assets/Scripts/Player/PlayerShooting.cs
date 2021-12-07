using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator[] handAnimators;
    [SerializeField] private GameObject hitMarker;

    [Header("Shooting variables")]
    [SerializeField] private float fireDistance = 200f;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Zoom varibles")]
    [SerializeField] private float unzoomedFOV;
    [SerializeField] private float zoomedFOV;
    [SerializeField] private float zoomSmoothing;

    [Header("Marker variables")]
    [SerializeField] private float markerLifeTime = 0.2f;

    private PlayerController player;

    private float timeSinceFire;
    private float targetFov;
    private float currentZoomSmoothVelocity;
    private int currentHandIndex;

    private void Awake()
    {
        this.player = GetComponent<PlayerController>();

        this.timeSinceFire = this.fireRate + 1f;
        this.targetFov = this.unzoomedFOV;
    }

    private void Start()
    {
        this.SetInputEvents();
    }

    private void SetInputEvents()
    {
        var input = player.Input;

        input.Movement.Fire.started += e => this.Fire();

        input.Movement.Zoom.performed += this.Zoom;
        input.Movement.Zoom.canceled += this.Zoom;

        input.Enable();
    }

    private void Update()
    {
        TimedVariables.UpdateTimeVariable(ref this.timeSinceFire, this.fireRate);

        this.mainCamera.fieldOfView = Mathf.SmoothDamp(this.mainCamera.fieldOfView, targetFov, ref this.currentZoomSmoothVelocity, this.zoomSmoothing);
    }

    private void Fire()
    {
        if (Pause.Paused)
            return;

        if (this.timeSinceFire < this.fireRate)
            return;
        this.timeSinceFire = 0f;

        this.handAnimators[this.currentHandIndex].SetTrigger("Fire");
        this.currentHandIndex++;
        if (this.currentHandIndex >= this.handAnimators.Length)
            this.currentHandIndex = 0;

        if (!Physics.Raycast(this.firePoint.position, this.firePoint.forward, out var hit, this.fireDistance))
            return;

        var marker = Instantiate(hitMarker, hit.point, hitMarker.transform.rotation);
        Destroy(marker, this.markerLifeTime);
    }

    private void Zoom(CallbackContext e)
    {
        if (Pause.Paused)
            return;

        this.targetFov = e.ReadValueAsButton() ? this.zoomedFOV : this.unzoomedFOV;
    }
}
