using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHQ : AttackableObject
{   
    [SerializeField] private Transform[] spawnLocs = new Transform[5];
    [SerializeField] private GameObject[] unitsToSpwn = new GameObject[5];

    private List<AttackableObject> playerUnits = null;

    [SerializeField] private int unitCapacity = 50;
    [SerializeField] private int currentUnits = 10;

    public override void Populate(int teamCode)
    {
        TeamCode = teamCode;
        CombatHandler.instance.AddUnit(this);

        gameObject.GetComponent<Renderer>().material.color = GameManager.instance.players[TeamCode].playerColor;

        //TeamCode = teamCode;
        healthBar.SetFillColor(GameManager.instance.players[teamCode].playerColor);
        //Debug.Log("Player colo");
        StartCoroutine(SpawnUnits());
    }

    IEnumerator SpawnUnits()
    {
        while (CombatHandler.instance == null) yield return new WaitForSeconds(1);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject g = Instantiate(unitsToSpwn[j], spawnLocs[j].position, spawnLocs[j].rotation);
                g.GetComponent<AttackableObject>().Populate(TeamCode);
                //Debug.Log("Creating unit of team " + TeamCode);
            }
            yield return new WaitForSeconds(3f);
        }
    }
}
