using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : AttackableObject
{
    public override void Populate(int teamCode)
    {
        CombatHandler.instance.AddResourceNode(this);
    }
}
