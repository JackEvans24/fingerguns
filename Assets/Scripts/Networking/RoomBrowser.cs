using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoomBrowser : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private TMP_Text noRoomsFound;
    [SerializeField] private CanvasGroup form;
    [SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private RectTransform roomListContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject roomItemTemplate;

    [Header("Variables")]
    [SerializeField] private int itemsInList;

    private void Awake()
    {
        for (int i = 0; i < this.itemsInList; i++)
        {
            var item = Instantiate(this.roomItemTemplate, this.roomListContainer);
            item.SetActive(false);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if (this.form.alpha > 0)
        {
            this.loadingCanvas.alpha = 1f;
            this.form.interactable = false;
        }

        this.noRoomsFound.gameObject.SetActive(false);
        foreach (Transform child in this.roomListContainer)
            child.gameObject.SetActive(false);

        var selectedRooms = roomList.Where(room => room.IsOpen && !room.RemovedFromList).ToList();
        if (selectedRooms.Count > this.itemsInList)
        {
            selectedRooms = roomList
                .OrderBy(room => Random.Range(0f, 1f))
                .Take(this.itemsInList)
                .ToList();
        }
        else if (selectedRooms.Count == 0)
        {
            this.noRoomsFound.gameObject.SetActive(true);

            this.form.interactable = true;
            this.loadingCanvas.alpha = 0f;

            return;
        }

        for (int i = 0; i <= selectedRooms.Count; i++)
        {
            var button = this.roomListContainer.GetChild(i).GetComponent<JoinRoomButton>();
            if (button == null)
                continue;

            button.SetUpButton(selectedRooms[i - 1]);
            button.gameObject.SetActive(true);
        }

        this.form.interactable = true;
        this.loadingCanvas.alpha = 0f;
    }
}
