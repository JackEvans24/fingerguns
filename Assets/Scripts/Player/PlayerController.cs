using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviour
{
    private PlayerActions input;
    public PlayerActions Input { get => this.input; }

    private PhotonView view;
    public PhotonView View { get => this.view; }

    private PlayerManager playerManager;

    private Health health;
    public Health Health { get => this.health; }

    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private MeshRenderer body;
    [SerializeField] private SkinnedMeshRenderer[] hands;
    [SerializeField] private Collider[] colliders;

    [Header("Pause")]
    [SerializeField] private CanvasGroup pauseCanvas;

    [Header("Death")]
    [SerializeField] private ParticleSystem[] deathParticles;
    [SerializeField] private float resetInterval;
    [SerializeField] private Vector3 deathCameraPosition;
    [SerializeField] private Vector3 deathCameraRotation;
    [SerializeField] private float cameraZoomInterval;

    public event EventHandler OnRemoveInputs;

    private void Awake()
    {
        this.input = new PlayerActions();

        this.health = GetComponent<Health>();
        this.view = GetComponent<PhotonView>();

        this.playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (this.view.IsMine)
        {
            this.cam.enabled = true;
            this.cam.gameObject.AddComponent<AudioListener>();

            input.Movement.Pause.started += this.TogglePause_Event;
            this.input.Enable();
        }
    }

    public void TogglePause_Event(InputAction.CallbackContext _e) => this.TogglePause();

    public void TogglePause()
    {
        Pause.TogglePause();

        this.pauseCanvas.interactable = Pause.Paused;
        this.pauseCanvas.alpha = Pause.Paused ? 1 : 0;

    }

    public void QuitGame()
    {
        this.OnRemoveInputs?.Invoke(this, null);
        this.input.Movement.Pause.started -= this.TogglePause_Event;

        Pause.LeaveGame();

        this.playerManager.Leave();
    }

    public void Die()
    {
        input.Disable();

        view.RPC(nameof(this.RPC_Die), RpcTarget.All);

        StartCoroutine(this.ResetAfterInterval());
    }

    [PunRPC]
    private void RPC_Die()
    {
        foreach (var col in this.GetComponents<Collider>())
            col.enabled = false;

        foreach (var collider in this.colliders)
            collider.enabled = false;

        body.enabled = false;
        foreach (var hand in hands)
            hand.enabled = false;

        foreach (var particles in this.deathParticles)
            particles.Play();

        this.cam.transform.DOLocalMove(this.deathCameraPosition, this.cameraZoomInterval).SetEase(Ease.OutSine);
        this.cam.transform.DOLocalRotate(this.deathCameraRotation, this.cameraZoomInterval).SetEase(Ease.OutSine);
    }

    private IEnumerator ResetAfterInterval()
    {
        yield return new WaitForSeconds(this.resetInterval);

        playerManager.Die();
    }
}
