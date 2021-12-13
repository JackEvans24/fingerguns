using Photon.Pun;
using System.Linq;
using UnityEngine;

public class Slap : MonoBehaviour
{
    [SerializeField] private PhotonView player;
    [SerializeField] private Collider[] myColliders;
    [SerializeField] private SoundFromArray slapNoise;
    [SerializeField] private int damage = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerBody"))
            return;
        if (this.myColliders.Contains(other))
            return;

        var healthCollider = other.GetComponent<HealthCollider>();
        if (healthCollider == null)
            return;

        this.player.RPC(nameof(this.RPC_Slap), RpcTarget.All);

        healthCollider.TakeDamage(this.damage, this.player.transform);
    }

    [PunRPC]
    private void RPC_Slap()
    {
        this.slapNoise.Play();
    }
}
