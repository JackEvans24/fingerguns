using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField createInput;
    [SerializeField] private InputField joinInput;

    public void CreateRoom()
    {
        // TODO: Use input text
        PhotonNetwork.CreateRoom("NewRoom");
    }

    public void JoinRoom()
    {
        // TODO: Use input text
        PhotonNetwork.JoinRoom("NewRoom");
    }

    public override void OnJoinedRoom()
    {
        // TODO: Use Index
        PhotonNetwork.LoadLevel("Game");
    }
}
