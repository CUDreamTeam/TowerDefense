using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance = null;

    private Dictionary<int, UnitBase> unitCodes = new Dictionary<int, UnitBase>();

    private int nextOccCode = 0;

    private void Awake()
    {
        instance = this;
    }

    public void RegisterUnit(UnitBase u)
    {
        u.occCode = nextOccCode;
        unitCodes[nextOccCode] = u;
        nextOccCode++;
        if(u.teamCode == 0) UnitSelection.instance.playerUnits.Add(u);
        //UnitSelection.instance.
    }

    public int GetNextOccCode()
    {
        nextOccCode++;
        return nextOccCode - 1;
    }

    public UnitBase GetUnitFromCode(int code)
    {
        return unitCodes[code];
    }
}
