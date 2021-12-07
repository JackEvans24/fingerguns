using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layers;
    [SerializeField] private LayerMask blockingLayers;

    [NonSerialized] public RaycastHit Hit;
    public bool HasHit { get => this.Hit.transform != null; }

    private void FixedUpdate()
    {
        Physics.Raycast(this.transform.position, this.transform.forward, out var hit, this.distance, this.layers + this.blockingLayers);

        if (hit.transform == null || (this.layers & (1 << hit.transform.gameObject.layer)) == 0)
            this.Hit = default;
        else
            this.Hit = hit;
    }

    private void OnDrawGizmosSelected()
    {
        var origin = this.transform.position;
        var destination = origin + (this.transform.forward * this.distance);

        Gizmos.DrawLine(origin, destination);
    }
}
