using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;

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

    [Header("Death")]
    [SerializeField] private ParticleSystem[] deathParticles;
    [SerializeField] private float resetInterval;
    [SerializeField] private Vector3 deathCameraPosition;
    [SerializeField] private Vector3 deathCameraRotation;
    [SerializeField] private float cameraZoomInterval;

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
        }
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
