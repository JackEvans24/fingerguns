using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine;

public class NicknameBillboard : MonoBehaviour
{
    [SerializeField] private PhotonView player;
    [SerializeField] private TMP_Text text;
    private Camera cam;

    private void Start()
    {
        this.text.text = player.Owner.NickName;
    }

    private void Update()
    {
        if (cam == null)
        {
            var sceneCameras = FindObjectsOfType<Camera>();
            cam = sceneCameras.FirstOrDefault(c => c.enabled);
        }

        if (cam == null)
            return;

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180f);
    }
}
