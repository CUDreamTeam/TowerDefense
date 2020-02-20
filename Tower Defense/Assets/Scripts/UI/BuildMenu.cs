using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    [SerializeField] private BuildingGhost turretGhost = null;
    [SerializeField] private Text turretText = null;
    [SerializeField] private float turretCost = 10f;
    [SerializeField] private BuildingGhost unitFactoryGhost = null;
    [SerializeField] private Text unitFactoryText = null;
    [SerializeField] private float unitFactoryCost = 100f;

    [SerializeField] private GameObject dropDownHolder = null;

    private int teamCode = 0;

    private PlayerInfo player = null;

    public void Populate(int TeamCode)
    {
        teamCode = TeamCode;
        player = GameManager.instance.players[teamCode];

        unitFactoryText.text = "Unit Factory: " + unitFactoryCost;
        turretText.text = "Turret: " + turretCost;
    }

    public void ToggleDropDownMenu()
    {
        dropDownHolder.SetActive(!dropDownHolder.activeSelf);
    }

    public void OnTurretClicked()
    {
        //BuildingPlacement.instance.ghost = Instantiate(turretGhost);
        //ToggleDropDownMenu();
        if (player.resources >= turretCost)
        {
            BuildingPlacement.instance.ghost = Instantiate(turretGhost);
            ToggleDropDownMenu();
            player.resources -= turretCost;
        }
    }

    public void OnUnitFactoryClicked()
    {
        if (player.resources >= unitFactoryCost)
        {
            BuildingPlacement.instance.ghost = Instantiate(unitFactoryGhost);
            ToggleDropDownMenu();
            player.resources -= unitFactoryCost;
        }
    }

    private void OnGUI()
    {
        turretText.color = player.resources >= turretCost ? Color.green : Color.red;
        unitFactoryText.color = player.resources >= unitFactoryCost ? Color.green : Color.red;
    }
}
