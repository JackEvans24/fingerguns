using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup loadingGroup;
    [SerializeField] private CanvasGroup joinRoomGroup;
    [SerializeField] private CanvasGroup regionGroup;
    [SerializeField] private CanvasGroup findRoomGroup;

    [Header("Menus")]
    [SerializeField] private RoomBrowser roomBrowser;

    [Header("Main Menu Inputs")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField codeInput;

    [SerializeField] private TMP_Text errorText;

    private bool changingRegion;

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
        this.ShowMainMenu();
    }

    public override void OnLeftLobby()
    {
        if (this.changingRegion)
            return;

        this.HideAllUI();

        this.loadingGroup.alpha = 1;

        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        if (string.IsNullOrWhiteSpace(this.codeInput.text))
        {
            this.errorText.text = "Room code invalid";
            return;
        }

        this.SetName();
        PhotonNetwork.CreateRoom(this.codeInput.text);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrWhiteSpace(this.codeInput.text))
        {
            this.errorText.text = "Room code invalid";
            return;
        }

        this.SetName();
        PhotonNetwork.JoinRoom(this.codeInput.text);
    }

    public void FindRoom()
    {
        this.SetName();

        this.HideAllUI();

        this.findRoomGroup.alpha = 1;
        this.findRoomGroup.blocksRaycasts = true;
        this.findRoomGroup.interactable = true;
    }

    public void ShowMainMenu()
    {
        this.HideAllUI();

        this.joinRoomGroup.alpha = 1;
        this.joinRoomGroup.blocksRaycasts = true;
        this.joinRoomGroup.interactable = true;
    }

    private void SetName()
    {
        var name = this.nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "Player";

        PhotonNetwork.NickName = name;
    }

    public void OpenRegionMenu()
    {
        this.changingRegion = true;

        this.HideAllUI();

        this.regionGroup.alpha = 1;
        this.regionGroup.blocksRaycasts = true;
        this.regionGroup.interactable = true;
    }

    public void CloseRegionMenu()
    {
        if (!this.changingRegion)
            return;

        this.changingRegion = false;

        this.ShowMainMenu();
    }

    private void HideAllUI()
    {
        this.loadingGroup.alpha = 0;

        this.joinRoomGroup.interactable = false;
        this.joinRoomGroup.blocksRaycasts = false;
        this.joinRoomGroup.alpha = 0;

        this.regionGroup.interactable = false;
        this.regionGroup.blocksRaycasts = false;
        this.regionGroup.alpha = 0;

        this.findRoomGroup.interactable = false;
        this.findRoomGroup.blocksRaycasts = false;
        this.findRoomGroup.alpha = 0;
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
        PhotonNetwork.LoadLevel("Game");
    }
}
