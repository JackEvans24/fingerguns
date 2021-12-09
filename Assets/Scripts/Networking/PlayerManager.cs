using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private PhotonView photonView;
    private GameObject player;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        this.player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, data: new object[] { photonView.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(this.player);
        this.CreatePlayer();
    }
}
