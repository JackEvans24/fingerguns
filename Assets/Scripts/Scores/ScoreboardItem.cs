using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text deathsText;

    public void Initialize(Player player)
    {
        this.usernameText.text = player.NickName;
    }

    public void UpdateScores(int kills, int deaths)
    {
        this.killsText.text = kills.ToString();
        this.deathsText.text = deaths.ToString();
    }
}
