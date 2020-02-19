using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : AttackableObject
{
    private void Start()
    {
        CombatHandler.instance.AddResourceNode(this);
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.changeHealth(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            //            Debug.Log("Killed unit");
            //CombatHandler.instance.RemoveUnit(this);
            CombatHandler.instance.RemoveResourceNode(this);
            Destroy(gameObject);
        }
        //Debug.Log("Resources left: " + currentHealth);
    }

    /*public override void Populate(int teamCode)
    {
        CombatHandler.instance.AddResourceNode(this);
    }*/
}
