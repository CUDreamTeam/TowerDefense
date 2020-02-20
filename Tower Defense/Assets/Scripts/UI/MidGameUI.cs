using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidGameUI : MonoBehaviour
{
    [SerializeField] private Text unitsText = null;
    [SerializeField] private Text buildingsText = null;
    [SerializeField] private Text resourcesText = null;

    [SerializeField] private BuildMenu buildMenu = null;
    [SerializeField] private UnitFactoryUI unitFactoryUI = null;

    private PlayerInfo player = null;

    public void Populate(ref PlayerInfo inf)
    {
        player = inf;
        buildMenu.Populate(inf.playerCode);
    }

    public void OpenUnitFactoryMenu(UnitFactory factory)
    {

    }

    private void OnGUI()
    {
        if (player == null)
        {
            unitsText.text = "Player is null";
            return;
        }
        unitsText.text = "Units: " + player.units.Count + " / " + player.unitCapacity;
        buildingsText.text = "Buildings: " + player.buildings.Count + " / " + player.buildingCapacity;
        resourcesText.text = "Resources: " + player.resources + " / " + player.resourceCapacity;
    }
}
