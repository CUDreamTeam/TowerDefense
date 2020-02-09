using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class UnitBase : MonoBehaviour, IComparable<UnitBase>
{
    //Movement variables
    public float moveSpeed = 10f;
    //Unique identifier used with dynamic object avoidence
    public int occCode = 0;

    public int size = 1;

    //UI variables
    //Used as selection marker
    //public bool selected = false;
    public HealthBar healthbar = null;

    //Combat variables
    public float currentHealth = 100;
    public float maxHealth = 100;
    public float timeBetweenScans = .5f;
    public int timeBetweenAttacks = 2000;
    public int maxScoutRange = 2;
    public int maxFireRange = 1;
    public float attackDamage = 10;
    Thread combatThread;

    bool needsNewTarget = false;
    bool isAttacking = false;

    //Inter unit identifiers
    public int playerCode = 0;
    public int teamCode = 0;

    //Status bools
    public bool isSelected = false;
    public bool isMoving = false;

    //Work around to getting position in a thread
    private Vector3 mPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        /*healthbar.gameObject.SetActive(false);
        StartCoroutine(FollowPath());
        StartCoroutine(CombatHandler());
        //UnitManager.instance.GetNextOccCode();

        if (teamCode == 0) healthbar.SetFillColor(Color.blue);
        else healthbar.SetFillColor(Color.red);*/
    }

    // Update is called once per frame
    void Update()
    {
        mPosition = transform.position;
    }

    public void PopulateUnit(int teamCode)
    {

    }

    public int CompareTo(UnitBase other)
    {
        return (size == other.size) ? 1 : (size > other.size) ? 1 : -1;
    }

    public void SetSelected(bool _isSelected)
    {
        if (healthbar.gameObject.activeSelf != _isSelected)
        {
            //print("Setting marker to " + b);
            isSelected = _isSelected;
            healthbar.gameObject.SetActive(isSelected);
        }
    }

    public List<Vector3> nvPath = new List<Vector3>();
    public Node[,] occupiedNodes = null;
    public int moveCode = -1;

    public Vector3 targetDest = Vector3.zero;

    IEnumerator FollowPath()
    {
        while (UnitManager.instance == null || UnitSelection.instance == null) yield return null;
        UnitManager.instance.RegisterUnit(this);

        Node[,] nextNodes = null;
        List<Vector3> vPath = new List<Vector3>();
        int pathIndex = 0;

        //Renderer mRenderer = gameObject.GetComponent<Renderer>();

        //while (occCode == -1) yield return null;
        while (MapManager.instance == null || !MapManager.instance.GetMapIsReady) yield return null;

        //Give the unit a starting position so that it will actually occupy map nodes
        //nvPath = new List<Vector3>();
        //nvPath.Add(GetAvgPosition(GetNodesFromLocation(transform.position)));
        vPath = new List<Vector3>();
        vPath.Add(GetAvgPosition(GetNodesFromLocation(transform.position)));
        occupiedNodes = GetNodesFromLocation(transform.position);
        FillNodes(occupiedNodes);

        //Debug.Log("Following path");

        if (targetDest != Vector3.zero)
        {
            RequestPath(targetDest, 1, 0);
        }

        /*foreach (Node n in occupiedNodes)
        {
            GameObject g = Instantiate(MapManager.instance.nodeMarkers[occCode]);
            g.transform.position = n.GetPosition;
            g.transform.localScale = MapManager.nodeSize;
        }*/

        while (true)
        {
            //If the unit has received a new path
            if (nvPath != null)
            {
                //Debug.Log("Attempting to use new path");
                if (nvPath.Count > 0)
                {
                    Node[,] startNodes = GetNodesFromLocation(nvPath[0]);
                    //Only use the new path if the new nodes are ok
                    if (NodesAreClear(startNodes))
                    {
                        //                        Debug.Log("Loading new path");
                        //Accept new path
                        vPath = nvPath;
                        pathIndex = 0;
                        nextNodes = startNodes;
                        nvPath = null;

                        //Add current location so only one section of code handles moving the unit
                        isMoving = true;
                        //Insert the current position to prevent issues with changing path
                        if (!vPath.Contains(transform.position)) vPath.Insert(0, transform.position);
                    }
                    else
                    {
                        Debug.Log("Rejecting new path");
                        Debug.Log("In way: " + GetUnitInWay(startNodes));
                        List<Vector3> tempVPath = nvPath;
                        nvPath = null;

                        isMoving = false;
                        //Check if blocked by unit or object
                        int inWay = GetUnitInWay(startNodes);

                        //If inway > 0 then there is a unit blocking path
                        if (inWay >= 0)
                        {
                            UnitBase u = UnitManager.instance.GetUnitFromCode(inWay);
                            while (u.isMoving && nvPath == null) yield return null;
                            if (nvPath != null) continue;
                            else
                            {
                                if (nextNodes != null && NodesAreClear(nextNodes))
                                {
                                    vPath = tempVPath;
                                    pathIndex = 0;
                                    nextNodes = startNodes;
                                    nvPath = null;

                                    //Add current location so only one section of code handles moving the unit
                                    isMoving = true;
                                    //Insert the current position to prevent issues with changing path
                                    if (!vPath.Contains(transform.position)) vPath.Insert(0, transform.position);
                                }
                            }
                        }
                        else
                        {
                            //Request new path based on nvPath
                            PathRequest req = new PathRequest()
                            {
                                start = MapManager.instance.GetNodeFromLocation(transform.position),
                                end = MapManager.instance.GetNodeFromLocation(tempVPath[tempVPath.Count - 1]),
                                requestee = this,
                                priority = 10,
                                specialCode = 1,
                                occCode = occCode
                            };
                        }
                    }
                }
                else
                {
                    nvPath = null;
                }
            }
            else if (transform.position != vPath[pathIndex])
            {
                transform.position = Vector3.MoveTowards(transform.position, vPath[pathIndex], moveSpeed * Time.deltaTime);
                isMoving = true;
                yield return null;
            }
            else if ((pathIndex + 1) < vPath.Count)
            {
                nextNodes = GetNodesFromLocation(vPath[pathIndex + 1]);
                if (!NodesAreClear(nextNodes))
                {
                    yield return null;
                    isMoving = false;
                    int inWay = GetUnitInWay(nextNodes);
                    if (inWay >= 0)
                    {
                        UnitBase u = UnitManager.instance.GetUnitFromCode(GetUnitInWay(nextNodes));
                        while (u.isMoving && nvPath == null && !NodesAreClear(nextNodes)) yield return new WaitForSeconds(0.25f);
                        if (nvPath != null)
                        {
                            continue;
                        }
                        else if (!u.isMoving)
                        {
//                            Debug.Log("Requesting new path, unit is in way");
                            //MapManager.instance.ReceivePathRequest(vPath[vPath.Count - 1], this, 1);
                            MapManager.instance.ReceivePathRequest(
                                new PathRequest{
                                start = MapManager.instance.GetNodeFromLocation(mPosition),
                                end = MapManager.instance.GetNodeFromLocation(vPath[vPath.Count - 1]),
                                requestee = this,
                                priority = 1,
                                specialCode = 1,
                                occCode = this.occCode
                            });
                            while (!u.isMoving && nvPath == null) yield return null;
                        }
                    }
                    else
                    {
                        //Inmoveable object in way, request a new path
                        //MapManager.instance.ReceivePathRequest(vPath[vPath.Count - 1], this, 1);
                        MapManager.instance.ReceivePathRequest(new PathRequest
                        {
                            start = MapManager.instance.GetNodeFromLocation(mPosition),
                            end = MapManager.instance.GetNodeFromLocation(vPath[vPath.Count - 1]),
                            requestee = this,
                            priority = 1,
                            specialCode = 1,
                            occCode = this.occCode
                        });
                        while (nvPath == null) yield return null;
                        continue;
                    }
                }
                else
                {
                    pathIndex += 1;
                    ClearNodes(occupiedNodes);
                    if (FillNodes(nextNodes))
                    {
                        occupiedNodes = nextNodes;
                    }
                    else
                    {
                        FillNodes(occupiedNodes);
                        pathIndex -= 1;
                    }
                }
            }
            else
            {
                isMoving = false;
                yield return null;
            }
        }
    }

    private int GetUnitInWay(Node[,] nodes)
    {
        //If no unit is present occCode = -1
        foreach (Node n in nodes) if (n.occCode != occCode && n.occCode != -1) return n.occCode;
        return -1;
    }

    bool NodesAreClear(Node[,] nodes)
    {
        foreach (Node n in nodes)
        {
            if(n == null)
            {
                Debug.Log("Node is null in nodes are clear");
                return false;
            }
            if (n.occCode != -1 && n.occCode != occCode)
            {
                return false;
            }
            if (!n.isWalkable)
            {
                return false;
            }
        }
        return true;
    }

    //Returns true if the nodes were populated
    private bool FillNodes(Node[,] nodes)
    {
        if (!NodesAreClear(nodes)) return false;
        foreach (Node n in nodes) n.occCode = occCode;
        return true;
    }

    private void ClearNodes(Node[,] nodes)
    {
        foreach (Node n in nodes) if (n != null && n.occCode == occCode) n.occCode = -1;
    }

    private Node[,] GetNodesFromLocation(Vector3 pos)
    {
        float length = MapManager.nodeLength;
        Node[,] toReturn = new Node[size, size];

        float x = pos.x - (size / 2) * MapManager.nodeLength;
        float z = pos.z - (size / 2) * MapManager.nodeLength;

        //Debug.Log("Getting nodes from: " + pos + " : x = " + x + ", z = " + z);

        for (int q = 0; q < size; q++)
        {
            for (int w = 0; w < size; w++)
            {
                if (toReturn[q, w] == null)
                {
                    Vector3 nPos = Vector3.zero;
                    nPos.y = pos.y;
                    nPos.x = x + (length * q);
                    nPos.z = z + (w * length);
                    toReturn[q, w] = MapManager.instance.GetNodeFromLocation(nPos);
//                    Debug.Log(nPos);
                }
            }
        }
        return toReturn;
    }

    private Vector3 GetAvgPosition(Node[,] nodes)
    {
        float x = ((nodes[0, 0].GetPosition.x + nodes[0, nodes.GetLength(nodes.Rank - 1) - 1].GetPosition.x) / 2) + MapManager.nodeLength / 2;
        float z = ((nodes[0, 0].GetPosition.z + nodes[nodes.GetLength(nodes.Rank - 1) - 1, 0].GetPosition.z) / 2) + MapManager.nodeLength / 2;
        return new Vector3(x - 0.01f, nodes[0, 0].GetPosition.y, z - 0.01f);
    }

    public void RequestPath(Vector3 target, int priority, int sCode)
    {
        //MapManager.instance.ReceivePathRequest(MakePathRequest(target, priority, sCode));
        MapManager.instance.ReceivePathRequest(new PathRequest
        {
            start = MapManager.instance.GetNodeFromLocation(mPosition),
            end = MapManager.instance.GetNodeFromLocation(target),
            requestee = this,
            priority = priority,
            specialCode = sCode,
            occCode = this.occCode
        });
    }

    IEnumerator CombatHandler()
    {
        UnitSearch search = null;
        while (MapManager.instance == null) yield return null;
        while (!MapManager.instance.GetMapIsReady) yield return null;
//        Debug.Log("Starting combat handler");
        //while (MapManager.mapNodes == null) yield return null;
        while (true)
        {
            if (search == null)
            {
                search = new UnitSearch(maxFireRange, occCode, teamCode, MapManager.instance.GetNodeFromLocation(transform.position));
                while (search.status == UnitSearch.SearchStatus.inProcess) yield return null;
                if (search.status == UnitSearch.SearchStatus.succeeded)
                {
                    if (search.result == null)
                    {
                        print("Search result is null");
                        continue;
                    }
                    search.result.TakeDamage(attackDamage);
                }
                search = null;
            }
            yield return new WaitForSeconds(timeBetweenScans);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthbar.changeHealth(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            print("Destroying unit");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ClearNodes(occupiedNodes);
    }
}
