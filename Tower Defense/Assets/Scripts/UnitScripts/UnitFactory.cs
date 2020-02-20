using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : AttackableObject
{
    private List<UnitBuildRequest> buildQueue = new List<UnitBuildRequest>();
    [SerializeField] private Transform spawnLoc = null;

    public override void Populate(int teamCode)
    {
        TeamCode = teamCode;
        CombatHandler.instance.AddUnit(this);

        gameObject.GetComponent<Renderer>().material.color = GameManager.instance.players[TeamCode].playerColor;
        healthBar.SetFillColor(GameManager.instance.players[TeamCode].playerColor);
        if (teamCode == 0) healthBar.gameObject.SetActive(false);

        isMovable = false;
        isBuilder = true;
    }

    public override void SetSelected(bool selected)
    {
        base.SetSelected(selected);
        if (selected) UnitFactoryUI.instance.Populate(this);
        else UnitFactoryUI.instance.Close();
    }

    //Use attack variables to build units
    public override void AttackTarget()
    {
        if (buildQueue.Count > 0 && Time.realtimeSinceStartup >= lastAttack + timeBetweenAttacks)
        {
            Debug.Log("Finished building unit");
            GameObject g = Instantiate(buildQueue[0].unitPrefab, spawnLoc.position, spawnLoc.rotation);
            g.GetComponent<AttackableObject>().Populate(TeamCode);
            buildQueue.RemoveAt(0);

            if (buildQueue.Count == 0)
            {
                isAttacking = false;
            }
            else
            {
                lastAttack = Time.realtimeSinceStartup;
                timeBetweenAttacks = buildQueue[0].buildTime;
            }
        }
    }

    public void AddBuildRequest(UnitBuildRequest req)
    {
        buildQueue.Add(req);
        if (isAttacking == false)
        {
            lastAttack = Time.realtimeSinceStartup;
            timeBetweenAttacks = buildQueue[0].buildTime;
            isAttacking = true;
        }
    }

    public PlayerInfo GetPlayerInfo()
    {
        return GameManager.instance.players[TeamCode];
    }
}

public class UnitBuildRequest
{
    public float buildTime = 0;
    public GameObject unitPrefab = null;
}