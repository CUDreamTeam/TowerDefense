using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance = null;
    public static Vector3 nodeSize = Vector3.one * 0.5f;
    public static Node[,] mapNodes = null;

    //For creation
    [SerializeField]
    private int xSize = 500;
    [SerializeField]
    private int zSize = 500;
    public int xNodes = 0;
    public int zNodes = 0;
    public static float nodeLength = 0.5f;

    //Path finding
    public List<PathRequest> pathRequests = new List<PathRequest>();

    //For use by other classes
    public bool GetMapIsReady { get; private set; } = false;

    //Debugging
    [SerializeField]
    public List<GameObject> nodeMarkers = new List<GameObject>();

    private Thread buildPathsThread = null;

    //Called before start
    private void Awake()
    {
        instance = this;
        StartCoroutine(CreateMap());
    }

    //Called every frame
    private void Update()
    {
        
    }

    private IEnumerator CreateMap()
    {
        Debug.Log("Creating map: " + (xSize * (1 / nodeLength)));
        xNodes = Mathf.FloorToInt(xSize * (1 / nodeLength));
        zNodes = Mathf.FloorToInt(zSize * (1 / nodeLength));

        CreateMap createMap = new CreateMap(nodeLength, xSize, zSize);
        while (!createMap.GetMapIsReady) yield return null;
        GetMapIsReady = true;
        mapNodes = createMap.mapNodes;

        Debug.Log((mapNodes.GetLength(0) - 1) + ", " + (mapNodes.GetLength(mapNodes.Rank - 1) - 1));

        GameObject g = Instantiate(nodeMarkers[0]);
        g.transform.position = mapNodes[0, 0].GetPosition;
        g.transform.localScale = nodeSize;
        g.transform.name = "[0, 0]";

        g = Instantiate(nodeMarkers[1]);
        g.transform.position = mapNodes[0, 999].GetPosition;
        g.transform.localScale = nodeSize;
        g.transform.name = "[0, " + (mapNodes.GetLength(mapNodes.Rank - 1) - 1) + "]";

        g = Instantiate(nodeMarkers[2]);
        g.transform.position = mapNodes[mapNodes.GetLength(0) - 1, 0].GetPosition;
        g.transform.localScale = nodeSize;
        g.transform.name = "[ " + (mapNodes.GetLength(0) - 1) + ", 0]";

        g = Instantiate(nodeMarkers[3]);
        g.transform.position = mapNodes[mapNodes.GetLength(0) - 1, mapNodes.GetLength(mapNodes.Rank - 1) - 1].GetPosition;
        g.transform.localScale = nodeSize;
        g.transform.name = "[ " + (mapNodes.GetLength(0) - 1) + ", " + (mapNodes.GetLength(mapNodes.Rank - 1) - 1) + "]";

        buildPathsThread = new Thread(new ThreadStart(BuildPaths));
        buildPathsThread.Start();
    }

    public Node GetNodeFromLocation(Vector3 loc)
    {
        float x = RoundFloat(loc.x * (1 / nodeLength));
        float z = RoundFloat(loc.z * (1 / nodeLength));

        //print("X size: " + xSize);
        //print("Z size: " + zSize);

        if (Mathf.FloorToInt(x) >= xSize * (1 / nodeLength) || Mathf.FloorToInt(z) >= zSize * (1 / nodeLength) || x < 0 || z < 0)
        {
            //print("Bad location " + loc);
            return null;
        }

        try
        {
            return mapNodes[Mathf.FloorToInt(x), Mathf.FloorToInt(z)];
        }
        catch
        {
            Debug.Log("Bad loc: (" + Mathf.FloorToInt(x) + ',' + Mathf.FloorToInt(z) + ") Total nodes: " + mapNodes.Length);
            return null;
        }
    }
    private float RoundFloat(float f)
    {
        if (f % 1 == 0)
        {
            return f;
        }
        else
        {
            float t = f % 1f;
            if (t >= .75f)
            {
                return (float)Math.Truncate(f) + 1;
            }
            else if (t >= .25f)
            {
                return (float)Math.Truncate(f) + .5f;
            }
            else
            {
                return (float)Math.Truncate(f);
            }
        }
    }

    public List<Node> GetNeighbors(Node n)
    {
        List<Node> neighbors = new List<Node>();
        int xIndex = n.xIndex;
        int zIndex = n.zIndex;

        if (xIndex < (xSize * (1 / nodeLength)) && xIndex >= 1 && zIndex < (zSize * (1 / nodeLength)) && zIndex >= 1)
        {
            neighbors.Add(mapNodes[xIndex - 1, zIndex - 1]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) && xIndex >= 0 && zIndex < (zSize * (1 / nodeLength)) - 1 && zIndex >= 0)
        {
            neighbors.Add(mapNodes[xIndex, zIndex + 1]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) && xIndex >= 0 && zIndex < (zSize * (1 / nodeLength)) && zIndex >= 1)
        {
            neighbors.Add(mapNodes[xIndex, zIndex - 1]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) - 1 && xIndex >= 1 && zIndex < (zSize * (1 / nodeLength)) && zIndex >= 0)
        {
            neighbors.Add(mapNodes[xIndex + 1, zIndex]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) && xIndex >= 1 && zIndex < (zSize * (1 / nodeLength)) && zIndex >= 0)
        {
            neighbors.Add(mapNodes[xIndex - 1, zIndex]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) - 1 && xIndex >= 0 && zIndex < (zSize * (1 / nodeLength)) - 1 && zIndex >= 0)
        {
            neighbors.Add(mapNodes[xIndex + 1, zIndex + 1]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) - 1 && xIndex >= 0 && zIndex < (zSize * (1 / nodeLength)) && zIndex >= 1)
        {
            neighbors.Add(mapNodes[xIndex + 1, zIndex - 1]);
        }
        if (xIndex < (xSize * (1 / nodeLength)) && xIndex >= 1 && zIndex < (zSize * (1 / nodeLength)) - 1 && zIndex >= 0)
        {
            neighbors.Add(mapNodes[xIndex - 1, zIndex + 1]);
        }
        return neighbors;
    }

    //Pathfinding functions
    public void ReceivePathRequest(PathRequest r)
    {
        pathRequests.Add(r);
    }
    /*public void ReceivePathRequest(Vector3 _targetPos, UnitBase _requestee, int moveCode)
    {
        pathRequests.Add(_requestee.MakePathRequest(_targetPos, 0, moveCode));
    }*/

    void BuildPaths()
    {
        while (!GetMapIsReady) ;
        //print("MapHandler: map is ready");
        while (true)
        {
            if (pathRequests.Count > 0)
            {
//                Debug.Log("Starting to build path");
                PathRequest req = pathRequests[0];
                if (req.specialCode == 0)
                {
                    PathFinder aStar = new PathFinder(req);
                    while (aStar.status == 0/* && aStar.thread.IsAlive*/) ;
                    if (aStar.status == 1)
                    {
                        req.requestee.nvPath = aStar.finalPath;
//                        Debug.Log("Sent new path to unit, size: " + aStar.finalPath.Count);
                    }
                    else
                    {
                        PlacementSearch p = new PlacementSearch(req.end, req.requestee.occCode, req.requestee.size);
                        while (p.status == 0);
                        req.end = MapManager.instance.GetNodeFromLocation(p.movePos[0]);
                        aStar = new PathFinder(req);
                        while (aStar.status == 0/* && aStar.thread.IsAlive*/) ;
                        if (aStar.status == 1)
                        {
                            req.requestee.nvPath = aStar.finalPath;
                            //                        Debug.Log("Sent new path to unit, size: " + aStar.finalPath.Count);
                        }
                        else
                        {
                            print("A start orig failed");
                        }
                    }
                    aStar = null;
                }
                else if (req.specialCode == 1)
                {
                    PathFinderA1 aStar = new PathFinderA1(req);
                    while (aStar.status == 0) ;
                    if (aStar.status == 1)
                    {
                        req.requestee.nvPath = aStar.vPath;
                    }
                    else
                    {
                        Debug.Log("A star a1 failed");
                    }
                    aStar = null;
                }
                pathRequests.RemoveAt(0);
//                Debug.Log("Finished build path");
            }
        }
    }
}

public struct PathRequest
{
    public Node start;
    public Node end;
    public UnitBase requestee;
    //Defualt to 0
    public int priority;
    public int specialCode;
    //public Action<List<Vector3>> callback;
    public int occCode;
}