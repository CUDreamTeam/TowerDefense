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
//                Debug.Log("Collecting partial: " + (capacity - collected));
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
//        if (TeamCode == 0) Debug.Log("Reached target");
        if (target == GameManager.instance.players[TeamCode].headQuarters)
        {
            PlayerInfo player = GameManager.instance.players[TeamCode];
            if (player.resources + collected > player.resourceCapacity)
            {
                player.resources += player.resourceCapacity - player.resources;
                collected -= player.resourceCapacity - player.resources;
                StartCoroutine(WaitToDeposit());
            }
            else
            {
                player.resources += collected;
                collected = 0;
                idealRange = collectRange;
                isAttacking = false;
                startSearch = true;
            }
        }
        else
        {
            idealRange = depositRange;
        }
    }

    IEnumerator WaitToDeposit()
    {
        Debug.Log("Waiting to deposit");
        PlayerInfo player = GameManager.instance.players[TeamCode];
        while (player.resources + collected > player.resourceCapacity) yield return null;
        Debug.Log("Can deposit");
        OnReachTarget();
    }
}
