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
    void Update()
    {
        agent.SetDestination(destination.position);
    }
}
