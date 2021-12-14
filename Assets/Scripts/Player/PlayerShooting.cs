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
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator[] handAnimators;
    [SerializeField] private SoundFromArray shootSound;
    [SerializeField] private SkinnedMeshRenderer[] hands;
    [SerializeField] private Transform slapHandTransform;
    [SerializeField] private SkinnedMeshRenderer slapHand;
    [SerializeField] private Collider slapCollider;
    [SerializeField] private SoundFromArray whooshSound;

    [Header("Shooting variables")]
    [SerializeField] private float fireDistance = 200f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float damage = 10;

    [Header("Zoom varibles")]
    [SerializeField] private float unzoomedFOV;
    [SerializeField] private float zoomedFOV;
    [SerializeField] private float zoomSmoothing;

    [Header("Melee variables")]
    [SerializeField] private float meleeDuration = 0.5f;
    [SerializeField] private float meleeRotation = -90f;
    [SerializeField] private float meleeHeightGain = 0.1f;
    [SerializeField] private float meleeRecovery = 1.5f;
    [SerializeField] private Ease meleeEase = Ease.InOutSine;


    private PlayerController player;

    private float timeSinceFire;
    private float timeSinceMelee;
    private float targetFov;
    private float currentZoomSmoothVelocity;
    private int currentHandIndex;
    private Vector3 initialSlapPosition, initialSlapRotation;

    private void Awake()
    {
        this.player = GetComponent<PlayerController>();

        this.timeSinceFire = this.fireRate + 1f;
        this.timeSinceMelee = this.meleeRecovery + 1f;

        this.targetFov = this.unzoomedFOV;

        this.initialSlapPosition = this.slapHandTransform.localPosition;
        this.initialSlapRotation = this.slapHandTransform.localRotation.eulerAngles;
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

        input.Movement.Fire.started += this.Fire;

        input.Movement.Melee.started += this.Melee;

        input.Movement.Zoom.performed += this.Zoom;
        input.Movement.Zoom.canceled += this.Zoom;

        this.player.OnRemoveInputs += (_s, _e) => this.RemoveInputEvents();
    }

    private void RemoveInputEvents()
    {
        var input = player.Input;

        input.Movement.Fire.started -= this.Fire;

        input.Movement.Melee.started -= this.Melee;

        input.Movement.Zoom.performed -= this.Zoom;
        input.Movement.Zoom.canceled -= this.Zoom;
    }

    private void Update()
    {
        if (!player.View.IsMine || this.player.Health.Dead)
            return;

        TimedVariables.UpdateTimeVariable(ref this.timeSinceFire, this.fireRate);
        TimedVariables.UpdateTimeVariable(ref this.timeSinceMelee, this.meleeRecovery);

        this.mainCamera.fieldOfView = Mathf.SmoothDamp(this.mainCamera.fieldOfView, targetFov, ref this.currentZoomSmoothVelocity, this.zoomSmoothing);
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
            return;

        this.targetFov = e.ReadValueAsButton() ? this.zoomedFOV : this.unzoomedFOV;
    }
}
