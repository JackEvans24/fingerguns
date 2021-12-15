using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator[] handAnimators;
    [SerializeField] private SoundFromArray shootSound;
    [SerializeField] private SkinnedMeshRenderer[] hands;
    [SerializeField] private Transform slapHandTransform;
    [SerializeField] private SkinnedMeshRenderer slapHand;
    [SerializeField] private Collider slapCollider;
    [SerializeField] private SoundFromArray whooshSound;

    [Header("View Variables")]
    [SerializeField] private Vector2 yViewClamp;
    [SerializeField] private float zoomedSensitivityModifier = 0.6f;

    [Header("Shooting variables")]
    [SerializeField] private float fireDistance = 200f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float damage = 10;

    [Header("Zoom varibles")]
    [SerializeField] private float unzoomedFOV = 60;
    [SerializeField] private float zoomedFOV = 30;
    [SerializeField] private float zoomSmoothing = 0.1f;

    [Header("Melee variables")]
    [SerializeField] private float meleeDuration = 0.5f;
    [SerializeField] private float meleeRotation = -90f;
    [SerializeField] private float meleeHeightGain = 0.1f;
    [SerializeField] private float meleeRecovery = 1.5f;
    [SerializeField] private Ease meleeEase = Ease.InOutSine;


    private PlayerController player;

    private Vector3 playerRotation;
    private Vector3 cameraRotation;
    private Vector2 viewInput;
    private int sensitivity;
    private float timeSinceFire;
    private float timeSinceMelee;
    private bool zoomed;
    private float currentZoomSmoothVelocity;
    private int currentHandIndex;
    private Vector3 initialSlapPosition, initialSlapRotation;

    private void Awake()
    {
        this.player = GetComponent<PlayerController>();

        this.playerRotation = this.transform.rotation.eulerAngles;
        this.cameraRotation = this.cameraTransform.rotation.eulerAngles;

        this.timeSinceFire = this.fireRate + 1f;
        this.timeSinceMelee = this.meleeRecovery + 1f;

        this.initialSlapPosition = this.slapHandTransform.localPosition;
        this.initialSlapRotation = this.slapHandTransform.localRotation.eulerAngles;
    }

    private void Start()
    {
        if (!player.View.IsMine)
            return;

        this.SetInputEvents();

        this.player.OnRemoveInputs += (_s, _e) => this.RemoveInputEvents();
        this.player.OnDie += (_s, _e) => this.HandleDeath();

        this.UpdateSettings(this, null);
    }

    private void SetInputEvents()
    {
        SettingsManager.OnSettingsChanged += this.UpdateSettings;

        var input = player.Input;

        input.Movement.View.performed += this.SetViewInput;
        input.Movement.View.canceled += this.SetViewInput;

        input.Movement.Fire.started += this.Fire;

        input.Movement.Melee.started += this.Melee;

        input.Movement.Zoom.performed += this.Zoom;
        input.Movement.Zoom.canceled += this.Zoom;
    }

    private void RemoveInputEvents()
    {
        SettingsManager.OnSettingsChanged -= this.UpdateSettings;

        var input = player.Input;

        input.Movement.View.performed -= this.SetViewInput;
        input.Movement.View.canceled -= this.SetViewInput;

        input.Movement.Fire.started -= this.Fire;

        input.Movement.Melee.started -= this.Melee;

        input.Movement.Zoom.performed -= this.Zoom;
        input.Movement.Zoom.canceled -= this.Zoom;
    }

    private void HandleDeath()
    {
        this.zoomed = false;
    }

    private void UpdateSettings(object sender, System.EventArgs e)
    {
        this.sensitivity = SettingsManager.Sensitivity;
    }

    private void SetViewInput(CallbackContext e) => this.viewInput = e.ReadValue<Vector2>() * 0.01f;

    private void Update()
    {
        if (!player.View.IsMine)
            return;

        if (this.player.Health.Dead || Pause.Paused)
            this.viewInput = Vector2.zero;

        this.SetView();

        TimedVariables.UpdateTimeVariable(ref this.timeSinceFire, this.fireRate);
        TimedVariables.UpdateTimeVariable(ref this.timeSinceMelee, this.meleeRecovery);

        var targetFov = this.zoomed ? this.zoomedFOV : this.unzoomedFOV;
        this.mainCamera.fieldOfView = Mathf.SmoothDamp(this.mainCamera.fieldOfView, targetFov, ref this.currentZoomSmoothVelocity, this.zoomSmoothing);
    }

    private void SetView()
    {
        this.cameraRotation.z = this.cameraTransform.rotation.eulerAngles.z;

        var totalSensitivity = this.sensitivity * (this.zoomed ? this.zoomedSensitivityModifier : 1);

        this.cameraRotation.x += -totalSensitivity * this.viewInput.y;
        this.cameraRotation.x = Mathf.Clamp(this.cameraRotation.x, this.yViewClamp.x, this.yViewClamp.y);

        this.cameraTransform.localRotation = Quaternion.Euler(this.cameraRotation);

        this.playerRotation.y += totalSensitivity * this.viewInput.x;
        this.transform.rotation = Quaternion.Euler(this.playerRotation);
    }

    private void Fire(CallbackContext e)
    {
        if (Pause.Paused || this.player.Health.Dead)
            return;

        if (this.timeSinceFire < this.fireRate || this.timeSinceMelee < this.meleeRecovery)
            return;
        this.timeSinceFire = 0f;

        this.player.View.RPC(nameof(this.RPC_Fire), RpcTarget.All);

        this.handAnimators[this.currentHandIndex].SetTrigger("Fire");
        this.currentHandIndex++;
        if (this.currentHandIndex >= this.handAnimators.Length)
            this.currentHandIndex = 0;

        if (!Physics.Raycast(this.firePoint.position, this.firePoint.forward, out var hit, this.fireDistance))
            return;

        var health = hit.collider.GetComponent<HealthCollider>();
        if (health == null)
            return;
        
        if (health.TryKill(this.damage, this.transform))
            this.AddKill();
    }

    [PunRPC]
    private void RPC_Fire()
    {
        this.shootSound.Play();
    }

    private void AddKill()
    {
        var properties = player.View.Owner.CustomProperties;

        if (properties.TryGetValue("Kills", out object storedKills))
            properties["Kills"] = (int)storedKills + 1;
        else
            properties.Add("Kills", 1);

        player.View.Owner.SetCustomProperties(properties);
    }

    private void Melee(CallbackContext e)
    {
        if (Pause.Paused || this.player.Health.Dead)
            return;

        if (this.timeSinceMelee < this.meleeRecovery)
            return;
        this.timeSinceMelee = 0f;

        this.player.View.RPC(nameof(this.RPC_Melee), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Melee()
    {
        StartCoroutine(Melee_Coroutine());
    }

    private IEnumerator Melee_Coroutine()
    {
        this.slapHandTransform.localPosition = this.initialSlapPosition;
        this.slapHandTransform.localRotation = Quaternion.Euler(this.initialSlapRotation);

        foreach (var hand in this.hands)
            hand.enabled = false;
        this.slapHand.enabled = true;
        this.slapCollider.enabled = true;

        this.slapHandTransform.DOLocalMoveY(this.slapHandTransform.localPosition.y + this.meleeHeightGain, this.meleeDuration).SetEase(this.meleeEase);
        this.slapHandTransform.DOLocalRotate(this.initialSlapRotation + (Vector3.up * this.meleeRotation), this.meleeDuration).SetEase(this.meleeEase);

        this.whooshSound.Play();

        yield return new WaitForSeconds(this.meleeDuration);

        this.slapCollider.enabled = false;
        this.slapHand.enabled = false;
        foreach (var hand in this.hands)
            hand.enabled = true;
    }

    private void Zoom(CallbackContext e)
    {
        if (Pause.Paused || this.player.Health.Dead)
            this.zoomed = false;
        else
            this.zoomed = e.ReadValueAsButton();
    }
}
