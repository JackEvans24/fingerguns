using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerActions input;
    public PlayerActions Input { get => this.input; }

    private void Awake()
    {
        input = new PlayerActions();
    }

    public void Die()
    {
        input.Disable();
    }
}
