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

    public float collectRange = 3f;
    public float depositRange = 5f;

    public override void Populate(int teamCode)
    {
        searchRange = float.MaxValue;
        base.Populate(teamCode);
        isMovable = true;
        isCollector = true;
        navAgent = GetComponent<NavMeshAgent>();

        idealRange = collectRange;
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
                Debug.Log("Collecting partial: " + (capacity - collected));
                target.TakeDamage(capacity - collected);
                collected = capacity;

                isAttacking = false;
                isApproaching = true;
                target = GameManager.instance.players[TeamCode].headQuarters;
            }
            else
            {
                target.TakeDamage(collectionAmount);
                collected += collectionAmount;
                if (collected >= capacity)
                {
                    isAttacking = false;
                    isApproaching = true;
                    target = GameManager.instance.players[TeamCode].headQuarters;
                }
            }
//            Debug.Log("Collected: " + collected);
            lastAttack = Time.realtimeSinceStartup;
        }
    }

    public override void OnReachTarget()
    {
        if (TeamCode == 0) Debug.Log("Reached target");
        if (target == GameManager.instance.players[TeamCode].headQuarters)
        {
            GameManager.instance.players[TeamCode].resources += collected;
            collected = 0;
            if (TeamCode == 0) Debug.Log("Collected resources");
            idealRange = collectRange;
            isAttacking = false;
            startSearch = true;
        }
        else
        {
            idealRange = depositRange;
        }
    }
}
