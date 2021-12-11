using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private CanvasGroup loadingGroup;
    [SerializeField] private CanvasGroup joinRoomGroup;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;

    [SerializeField] private TMP_Text errorText;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        this.loadingGroup.alpha = 0;
        this.joinRoomGroup.alpha = 1;
        this.joinRoomGroup.interactable = true;
    }

    public override void OnLeftLobby()
    {
        this.joinRoomGroup.interactable = false;
        this.joinRoomGroup.alpha = 0;
        this.loadingGroup.alpha = 1;

        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        if (string.IsNullOrWhiteSpace(this.createInput.text))
        {
            this.errorText.text = "Room code invalid";
            return;
        }

        this.SetName();
        PhotonNetwork.CreateRoom(this.createInput.text);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrWhiteSpace(this.joinInput.text))
        {
            this.errorText.text = "Room code invalid";
            return;
        }

        this.SetName();
        PhotonNetwork.JoinRoom(this.joinInput.text);
    }

    private void SetName()
    {
        var name = this.nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "Player";

        PhotonNetwork.NickName = name;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        this.errorText.text = "Create room failed: " + message;
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        this.errorText.text = "Join room failed: " + message;
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        // TODO: Use Index
        PhotonNetwork.LoadLevel("Game");
    }
}
