using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    [SerializeField] private BuildingGhost turretGhost = null;
    [SerializeField] private BuildingGhost unitFactoryGhost = null;

    [SerializeField] private GameObject dropDownHolder = null;

    private int teamCode = 0;

    public void Populate(int TeamCode)
    {
        teamCode = TeamCode;
    }

    public void ToggleDropDownMenu()
    {
        dropDownHolder.SetActive(!dropDownHolder.activeSelf);
    }

    public void OnTurretClicked()
    {
        BuildingPlacement.instance.ghost = Instantiate(turretGhost);
        ToggleDropDownMenu();
    }

    public void OnUnitFactoryClicked()
    {
        BuildingPlacement.instance.ghost = Instantiate(unitFactoryGhost);
        ToggleDropDownMenu();
    }
}
