using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargedAttackUnit : ChargedAttackBase
{
    public override void Populate(int teamCode)
    {
        base.Populate(teamCode);
        isMovable = true;
        canAttack = true;
    }
}
