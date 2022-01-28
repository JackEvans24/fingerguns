using Photon.Pun;
using TMPro;
using UnityEngine;

public class RegionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private CanvasGroup regionCanvas;
    [SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private TMP_Text regionText;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private TMP_Dropdown regionDropdown;

    private Regions currentRegion = Regions.Auto;

    public void UpdateRegion()
    {
        this.currentRegion = (Regions)this.regionDropdown.value;
    }

    public void Connect()
    {
        this.regionCanvas.interactable = false;
        this.loadingCanvas.alpha = 1;

        PhotonNetwork.Disconnect();

        if (this.currentRegion == Regions.Auto)
        {
            var bestRegionSummary = PhotonNetwork.BestRegionSummaryInPreferences;
            var bestRegion = bestRegionSummary.Substring(0, bestRegionSummary.IndexOf(";"));
            PhotonNetwork.ConnectToRegion(bestRegion);
        }
        else
            PhotonNetwork.ConnectToRegion(this.currentRegion.GetRegionCode());
    }

    public override void OnConnectedToMaster()
    {
        this.loadingCanvas.alpha = 0;

        base.OnConnectedToMaster();

        this.SetRegionText(PhotonNetwork.CloudRegion);
        this.roomManager.CloseRegionMenu();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        this.SetRegionText(PhotonNetwork.CloudRegion);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        this.SetRegionText(string.Empty);
    }

    public void Cancel()
    {
        this.roomManager.CloseRegionMenu();
    }

    private void SetRegionText(string region)
    {
        if (region == string.Empty)
            this.regionText.text = string.Empty;
        else
        {
            if (region.Contains("/"))
                region = region.Substring(0, region.IndexOf("/"));
            this.regionText.text = $"Current Region: {region}";
        }
    }
}
