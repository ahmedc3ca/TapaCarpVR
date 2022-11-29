using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkerController : MonoBehaviour
{
    [SerializeField]
    private Transform destination;

    public NavMeshAgent agent;
    // Update is called once per frame
    private bool going = false;

    public void GoToDestination()
    {
        going = true;
    }
    void Update()
    {
        if (!going) return;
        agent.SetDestination(destination.position);
    }
}
