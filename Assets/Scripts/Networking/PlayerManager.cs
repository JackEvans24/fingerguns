using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private PhotonView photonView;
    private GameObject player;

    private static int currentSpawnPointIndex;

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
        var spawnPoint = this.spawnPoints[currentSpawnPointIndex];
        this.player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, data: new object[] { photonView.ViewID });

        currentSpawnPointIndex++;
        if (currentSpawnPointIndex >= this.spawnPoints.Length)
            currentSpawnPointIndex = 0;
    }

    public void Die()
    {
        PhotonNetwork.Destroy(this.player);
        this.CreatePlayer();
    }
}
