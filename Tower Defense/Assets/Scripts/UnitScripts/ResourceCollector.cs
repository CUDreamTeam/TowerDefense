using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResourceCollector : AttackableObject
{
    public float collected = 0;
    public float capacity = 10f;
    public float collectionSpeed = 0.1f;
    public float collectionAmount = 0.1f;

    public float lastCollectionTime = 0;

    public override void Populate(int teamCode)
    {
        base.Populate(teamCode);
        isMovable = true;
        isCollector = true;
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void CollectResources()
    {
        if (Time.realtimeSinceStartup >= lastCollectionTime + collectionSpeed)
        {
            lastCollectionTime = Time.realtimeSinceStartup;
            capacity += collectionAmount;
        }
    }
}
