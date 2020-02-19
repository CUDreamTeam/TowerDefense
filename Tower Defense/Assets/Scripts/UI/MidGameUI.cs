using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidGameUI : MonoBehaviour
{
    [SerializeField] private Text unitsText = null;
    [SerializeField] private Text resourcesText = null;

    private PlayerInfo player = null;

    public void Populate(ref PlayerInfo inf)
    {
        player = inf;
    }

    private void OnGUI()
    {
        if (player == null)
        {
            unitsText.text = "Player is null";
            return;
        }
        unitsText.text = "Units: " + player.attackableObjects.Count + " / " + player.unitCapacity;
        resourcesText.text = "Resources: " + player.resources + " / " + player.resourceCapacity;
    }
}
