using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    [SerializeField] private float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        PhotonNetwork.Destroy(this.gameObject);
    }
}
