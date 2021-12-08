using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviour
{
    private PlayerActions input;
    public PlayerActions Input { get => this.input; }

    private PhotonView view;
    public PhotonView View { get => this.view; }

    [SerializeField] private Camera cam;

    private void Awake()
    {
        input = new PlayerActions();

        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (this.view.IsMine)
            cam.enabled = true;
    }

    public void Die()
    {
        input.Disable();
    }
}
