using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Events;
public class WalkerController : MonoBehaviour
{
    [SerializeField]
    private Transform destination;

    public NavMeshAgent agent;

    public ThirdPersonCharacter character;

    public UnityEvent arrived;
    // Update is called once per frame
    private bool going = false;

    private void Start()
    {
        agent.updateRotation = false;
    }
    public void GoToDestination()
    {
        going = true;
    }
    void Update()
    {
        if (!going) return;
        agent.SetDestination(destination.position);

        if(agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 31)
        {
            arrived.Invoke();
        }
    }
}
