using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This needs to display build options and builds inprogress
public class UnitFactoryUI : MonoBehaviour
{
    public static UnitFactoryUI instance = null;
    //Factory that triggered UI
    private UnitFactory factory = null;

    [SerializeField] private Button collectorButton = null;
    [SerializeField] private GameObject collectorPrefab = null;
    [SerializeField] private float collectorCost = 10f;
    [SerializeField] private float collectorBuildTime = 10f;
    [SerializeField] private Button engineerButton = null;
    [SerializeField] private GameObject engineerPrefab = null;
    [SerializeField] private float engineerCost = 10f;
    [SerializeField] private float engineerBuildTime = 10f;
    [SerializeField] private Button meleeButton = null;
    [SerializeField] private GameObject meleePrefab = null;
    [SerializeField] private float meleeCost = 10f;
    [SerializeField] private float meleeBuildTime = 10f;
    [SerializeField] private Button rangedButton = null;
    [SerializeField] private GameObject rangedPrefab = null;
    [SerializeField] private float rangedCost = 10f;
    [SerializeField] private float rangedBuildTime = 10f;

    private void Awake()
    {
        instance = this;
        Close();
    }

    private void Start()
    {
        collectorButton.onClick.AddListener(delegate { AddCollector(); });
        engineerButton.onClick.AddListener(delegate { AddEngineer(); });
        meleeButton.onClick.AddListener(delegate { AddMelee(); });
        rangedButton.onClick.AddListener(delegate { AddRanged(); });
    }

    public void Populate(UnitFactory Factory)
    {
        factory = Factory;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void AddCollector()
    {
        Debug.Log("Adding collector");
        PlayerInfo temp = factory.GetPlayerInfo();
        if (temp.resources >= collectorCost)
        {
            factory.AddBuildRequest(new UnitBuildRequest()
            {
                buildTime = collectorBuildTime,
                unitPrefab = collectorPrefab
            });
            temp.resources -= collectorCost;
        }
    }

    private void AddEngineer()
    {
        Debug.Log("Adding engineer");
        PlayerInfo temp = factory.GetPlayerInfo();
        if (temp.resources >= engineerCost)
        {
            factory.AddBuildRequest(new UnitBuildRequest()
            {
                buildTime = engineerBuildTime,
                unitPrefab = engineerPrefab
            });
            temp.resources -= engineerCost;
        }
    }

    private void AddMelee()
    {
        Debug.Log("Adding melee");
        PlayerInfo temp = factory.GetPlayerInfo();
        if (temp.resources >= meleeCost)
        {
            factory.AddBuildRequest(new UnitBuildRequest()
            {
                buildTime = meleeBuildTime,
                unitPrefab = meleePrefab
            });
            temp.resources -= meleeCost;
        }
    }

    private void AddRanged()
    {
        Debug.Log("Adding ranged");
        PlayerInfo temp = factory.GetPlayerInfo();
        if (temp.resources >= rangedCost)
        {
            factory.AddBuildRequest(new UnitBuildRequest()
            {
                buildTime = rangedBuildTime,
                unitPrefab = rangedPrefab
            });
            temp.resources -= rangedCost;
        }
    }
}
