using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TriggerTracker : MonoBehaviour
{

    public Material collidingMaterial;
    public Material defaultMaterial;

    public TeleportButton tb;

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<MeshRenderer>().material = collidingMaterial;
        tb.DisableButton();
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
        tb.EnableButton();
    }
}
