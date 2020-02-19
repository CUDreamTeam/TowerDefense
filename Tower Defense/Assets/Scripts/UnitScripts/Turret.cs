using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : AttackableObject
{
    public override void Populate(int teamCode)
    {
        TeamCode = teamCode;
        CombatHandler.instance.AddUnit(this);
        startSearch = true;

        gameObject.GetComponent<Renderer>().material.color = GameManager.instance.players[TeamCode].playerColor;
        healthBar.SetFillColor(GameManager.instance.players[TeamCode].playerColor);
        if (teamCode == 0) healthBar.gameObject.SetActive(false);

        isMovable = false;
        canAttack = true;
    }
}
