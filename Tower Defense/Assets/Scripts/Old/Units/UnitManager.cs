using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KdTree;
using KdTree.Math;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance = null;

    private Dictionary<int, UnitBase> unitCodes = new Dictionary<int, UnitBase>();
    private List<int> playerCodes = new List<int>();

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

    KdTree<float, GameObject> staticObjects = new KdTree<float, GameObject>(2, new FloatMath());

    IEnumerator CombatHandler()
    {
        while (true)
        {
            for(int i = 0; i < playerCodes.Count; i++)
            {
                KdTree<float, UnitBase> attackable = new KdTree<float, UnitBase>(2, new FloatMath());
                for (int j = 0; j < playerCodes.Count; i++)
                {
                    //If other team
                    if (j != i)
                    {

                    }
                }
            }
        }
    }
}
