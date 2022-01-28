using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class JoinRoomButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private TMP_Text playersText;

    private Button button;
    private RoomInfo roomInfo;

    private void Awake()
    {
        this.button = GetComponent<Button>();

        button.onClick.AddListener(JoinRoom);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetUpButton(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;

        this.codeText.text = $"Code: {roomInfo.Name}";
        this.playersText.text = $"Players: {roomInfo.PlayerCount}";
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(this.roomInfo.Name);
    }
}
