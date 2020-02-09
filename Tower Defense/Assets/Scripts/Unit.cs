using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : AttackableObject
{
    public void Populate(int teamCode)
    {
        TeamCode = teamCode;
        CombatHandler.instance.AddUnit(this);
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(transform.position);
        startSearch = true;
        isMovable = true;

        gameObject.GetComponent<Renderer>().material.color = GameManager.instance.players[TeamCode].playerColor;
    }
}
