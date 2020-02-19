using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField] private MidGameUI midgameUI = null;

    private void Awake()
    {
        instance = this;
    }

    public void Populate(ref PlayerInfo playerInf)
    {
//        Debug.Log("Populating UI");
        midgameUI.Populate(ref playerInf);
    }
}
