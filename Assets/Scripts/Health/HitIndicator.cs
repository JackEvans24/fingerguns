using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private float fadeTime;

    private float currentFadeTime;

    private void Awake()
    {
        this.currentFadeTime = this.fadeTime + 1;
    }

    private void Update()
    {
        TimedVariables.UpdateTimeVariable(ref this.currentFadeTime, this.fadeTime);

        if (this.currentFadeTime <= this.fadeTime)
            this.canvas.alpha = 1 - (this.currentFadeTime / this.fadeTime);
        else if (this.canvas.alpha != 0)
            this.canvas.alpha = 0;

    }

    public void ShowHitFrom(Vector3 damagePosition)
    {
        var angle = Vector3.SignedAngle(damagePosition - player.position, player.forward, player.up);

        var rotation = this.canvas.transform.rotation.eulerAngles;
        this.canvas.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, angle);

        this.currentFadeTime = 0;
        this.canvas.alpha = 1;
    }
}
