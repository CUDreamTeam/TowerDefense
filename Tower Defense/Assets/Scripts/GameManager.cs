using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public int numTeamsToCreate = 1;
    [SerializeField] private List<Transform> spawnLocs = new List<Transform>();

    [SerializeField] private GameObject playerHQ = null;

    public List<PlayerInfo> players = new List<PlayerInfo>();

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
            players.Add(new PlayerInfo()
            {
                playerCode = 0,
                playerColor = temp
            });
            GameObject g = Instantiate(playerHQ, spawnLocs[i].position, spawnLocs[i].rotation);
            g.GetComponent<PlayerHQ>().Populate(i);
        }
    }
}

[SerializeField]
public class PlayerInfo
{
    public int playerCode = 0;
    public List<AttackableObject> attackableObjects = new List<AttackableObject>();
    public Color playerColor = Color.green;
    public PlayerHQ headQuarters = null;
}

public class TeamInfo
{
    public int playerCode = 0;
    public List<int> allies = new List<int>();
    public List<AttackableObject> attackable = new List<AttackableObject>();
}