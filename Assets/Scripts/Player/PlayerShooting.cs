using Photon.Pun;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator[] handAnimators;

    [Header("Shooting variables")]
    [SerializeField] private float fireDistance = 200f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float damage = 10;

    [Header("Zoom varibles")]
    [SerializeField] private float unzoomedFOV;
    [SerializeField] private float zoomedFOV;
    [SerializeField] private float zoomSmoothing;

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
        if (!player.View.IsMine)
            return;

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
        if (!player.View.IsMine || this.player.Health.Dead)
            return;

        TimedVariables.UpdateTimeVariable(ref this.timeSinceFire, this.fireRate);

        this.mainCamera.fieldOfView = Mathf.SmoothDamp(this.mainCamera.fieldOfView, targetFov, ref this.currentZoomSmoothVelocity, this.zoomSmoothing);
    }

    private void Fire()
    {
        if (Pause.Paused || this.player.Health.Dead)
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

        var health = hit.collider.GetComponent<HealthCollider>();
        if (health != null)
            health.TakeDamage(this.damage, this.transform);
    }

    private void Zoom(CallbackContext e)
    {
        if (Pause.Paused || this.player.Health.Dead)
            return;

        this.targetFov = e.ReadValueAsButton() ? this.zoomedFOV : this.unzoomedFOV;
    }
}
