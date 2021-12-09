using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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

    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private MeshRenderer body;
    [SerializeField] private SkinnedMeshRenderer[] hands;

    [Header("Death")]
    [SerializeField] private ParticleSystem[] deathParticles;
    [SerializeField] private float resetInterval;

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
        body.enabled = false;
        foreach (var hand in hands)
            hand.enabled = false;

        foreach (var particles in this.deathParticles)
            particles.Play();
    }

    private IEnumerator ResetAfterInterval()
    {
        yield return new WaitForSeconds(this.resetInterval);

        playerManager.Die();
    }
}
