using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private PhotonView photonView;
    private GameObject player;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        var spawnPoint = this.GetSpawnPoint();
        this.player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, data: new object[] { photonView.ViewID });
    }

    public void Leave()
    {
        PhotonNetwork.Destroy(this.player);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public Transform GetSpawnPoint() => this.spawnPoints[Random.Range(0, this.spawnPoints.Length)];
}
