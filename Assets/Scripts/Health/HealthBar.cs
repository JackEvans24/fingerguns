using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Canvas hud;
    [SerializeField] private Slider primarySlider;

    [SerializeField] private float transitionDuration;
    [SerializeField] private AnimationCurve transitionCurve;

    private IEnumerator transition;

    private void Start()
    {
        if (!player.View.IsMine)
            this.hud.enabled = false;
    }

    public void ResetHealth(Health health)
    {
        primarySlider.maxValue = health.MaxHealth;
        primarySlider.value = health.CurrentHealth;
    }

    public void UpdateHealth(Health health)
    {
        if (!player.View.IsMine)
            return;

        if (this.transition != null)
            StopCoroutine(this.transition);

        this.transition = this.UpdateHealthBar(health.CurrentHealth);
        StartCoroutine(this.transition);
    }

    private IEnumerator UpdateHealthBar(float newHealth)
    {
        var startingHealth = primarySlider.value;
        var difference = startingHealth - newHealth;
        var currentTransitionTime = 0f;

        while (this.primarySlider.value > newHealth)
        {
            this.primarySlider.value = startingHealth - (difference * this.transitionCurve.Evaluate(currentTransitionTime / this.transitionDuration));

            yield return new WaitForEndOfFrame();
            currentTransitionTime += Time.deltaTime;
        }

        this.transition = null;
    }
}
