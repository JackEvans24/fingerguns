using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerController player;
    [SerializeField] private CanvasGroup scoreboard;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject scoreboardItem;

    private Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    private void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
            this.OnPlayerPropertiesUpdate(player, player.CustomProperties);
        }

        if (this.player.View.IsMine)
            this.SetInputEvents();
    }

    private void SetInputEvents()
    {
        var input = this.player.Input;

        input.Movement.Scores.performed += this.ShowScores;
        input.Movement.Scores.canceled += this.ShowScores;

        this.player.OnRemoveInputs += (_s, _e) => this.RemoveInputEvents();
    }

    private void RemoveInputEvents()
    {
        var input = this.player.Input;

        input.Movement.Scores.performed -= this.ShowScores;
        input.Movement.Scores.canceled -= this.ShowScores;
    }

    private void ShowScores(InputAction.CallbackContext e)
    {
        if (Pause.Paused)
            return;

        this.scoreboard.alpha = e.ReadValueAsButton() ? 1 : 0;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        this.AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.RemoveScoreboardItem(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!this.scoreboardItems.TryGetValue(targetPlayer, out var item))
            return;

        var kills = (int)changedProps["Kills"];
        var deaths = (int)changedProps["Deaths"];

        item.UpdateScores(kills, deaths);
    }

    private void AddScoreboardItem(Player player)
    {
        var item = Instantiate(this.scoreboardItem, this.container).GetComponent<ScoreboardItem>();
        item.Initialize(player);

        this.scoreboardItems[player] = item;
    }

    private void RemoveScoreboardItem(Player player)
    {
        Destroy(this.scoreboardItems[player].gameObject);
        this.scoreboardItems.Remove(player);
    }
}
