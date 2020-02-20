using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public int numTeamsToCreate = 1;
    [SerializeField] private List<Transform> spawnLocs = new List<Transform>();

    [SerializeField] private GameObject playerHQ = null;

    //public List<PlayerInfo> players = new List<PlayerInfo>();
    public PlayerInfo[] players = new PlayerInfo[20];

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < numTeamsToCreate; i++)
        {
            //Color temp = new Color32((Random.Range(0, 255)) / 255, Random.Range(0, 255) / 255, Random.Range(0, 255) / 255, 1);
            Color temp = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            //Debug.Log("Temp: " + temp);
            PlayerInfo player = new PlayerInfo()
            {
                playerCode = 0,
                playerColor = temp
            };
            players[i] = player;
            GameObject g = Instantiate(playerHQ, spawnLocs[i].position, spawnLocs[i].rotation);
            g.GetComponent<PlayerHQ>().Populate(i);
        }
        UIManager.instance.Populate(ref players[0]);
    }
}

[SerializeField]
public class PlayerInfo
{
    public int playerCode = 0;
    public Color playerColor = Color.green;
    public PlayerHQ headQuarters = null;

    public int unitCapacity = 10;
    public int buildingCapacity = 10;
    public List<AttackableObject> attackableObjects = new List<AttackableObject>();
    public List<AttackableObject> units = new List<AttackableObject>();
    public List<AttackableObject> buildings = new List<AttackableObject>();

    public float resources = 0f;
    public float resourceCapacity = 1000f;
}

public class TeamInfo
{
    public int playerCode = 0;
    public List<int> allies = new List<int>();
    public List<AttackableObject> attackable = new List<AttackableObject>();
}