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
        searchRange = float.MaxValue;
        base.Populate(teamCode);
        isMovable = true;
        isCollector = true;
        navAgent = GetComponent<NavMeshAgent>();
    }

    /*public void CollectResources()
    {
        if (Time.realtimeSinceStartup >= lastCollectionTime + collectionSpeed)
        {
            lastCollectionTime = Time.realtimeSinceStartup;
            capacity += collectionAmount;
        }
    }*/

    public override void AttackTarget()
    {
        if (Time.realtimeSinceStartup >= lastAttack + timeBetweenAttacks)
        {
            if (collectionAmount + collected >= capacity)
            {
                target.TakeDamage(collected - collectionAmount);
                collected = capacity;

                isAttacking = false;
                isApproaching = true;
                target = GameManager.instance.players[TeamCode].headQuarters;
            }
            else
            {
                target.TakeDamage(collectionAmount);
                collected += collectionAmount;
            }
            Debug.Log("Collected: " + collected);
            lastAttack = Time.realtimeSinceStartup;
        }
    }
}
