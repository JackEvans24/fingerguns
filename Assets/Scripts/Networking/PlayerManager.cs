using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerManager : MonoBehaviour
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
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        var spawnPoint = this.spawnPoints[Random.Range(0, spawnPoints.Length)];
        this.player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, data: new object[] { photonView.ViewID });

    }

    public void Die()
    {
        PhotonNetwork.Destroy(this.player);
        this.CreatePlayer();
    }
}
