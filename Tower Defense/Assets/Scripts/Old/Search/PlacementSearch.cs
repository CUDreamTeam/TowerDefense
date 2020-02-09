using System.Threading;
using System.Collections.Generic;
using UnityEngine;

/*
 * Notes
 * 1. Change actual search to not stop between units
 * 2. Reset only touched nodes after search
 * */

public class PlacementSearch
{
    private List<UnitBase> unitsToPlace = null;
    private Node start = null;
    public List<Vector3> movePos = new List<Vector3>();
    public int status = 0;

    private HashSet<Node> claimed = null;

    public PlacementSearch(List<UnitBase> _unitsToPlace, Node _start)
    {
        unitsToPlace = _unitsToPlace;
        start = _start;

        Thread temp = new Thread(new ThreadStart(Search));
        temp.Start();
    }

    /// <summary>
    /// Used by pathfinders to find new goal when current is bad
    /// </summary>
    /// <param name="start"></param>
    /// <param name="code"></param>
    public PlacementSearch(Node start, int code, int size)
    {
        claimed = new HashSet<Node>();

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        //HashSet<Node> claimed = new HashSet<Node>();
        bool found = false;

        openSet.Add(start);
        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(current);

            Node[,] nodesToTest = GetNodesFromLocationV3(current.GetPosition, (int)(1.5 * (size * (1 / MapManager.nodeLength))));

            if (NodesAreOK(nodesToTest))
            {
                foreach (Node n in nodesToTest)
                {
                    //n.claimed = true;
                    claimed.Add(n);
                }
                movePos.Add(GetAvgPosition(nodesToTest));
                found = true;
                break;
            }

            foreach (Node n in MapManager.instance.GetNeighbors(current))
            {
                if (!closedSet.Contains(n))
                {
                    if (!openSet.Contains(n))
                    {
                        //Debug.Log("Added node");
                        openSet.Add(n);
                    }
                }
            }
        }
        if (!found)
        {
            //Debug.Log("Open set: " + openSet.Count + ", Closed set: " + closedSet.Count);
            status = 2;
            return;
        }
        status = 1;
    }

    void Search()
    {
        //ResetNodes();
        claimed = new HashSet<Node>();

        for (int i = 0; i < unitsToPlace.Count; i++)
        {
            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();
            //HashSet<Node> claimed = new HashSet<Node>();
            bool found = false;

            openSet.Add(start);
            while (openSet.Count > 0)
            {
                Node current = openSet[0];
                openSet.RemoveAt(0);
                closedSet.Add(current);

                Node[,] nodesToTest = GetNodesFromLocationV3(current.GetPosition, (int)(1.5 * (unitsToPlace[i].size * (1 / MapManager.nodeLength))));

                if (NodesAreOK(nodesToTest))
                {
                    foreach (Node n in nodesToTest)
                    {
                        //n.claimed = true;
                        claimed.Add(n);
                    }
                    movePos.Add(GetAvgPosition(nodesToTest));
                    found = true;
                    break;
                }

                foreach (Node n in MapManager.instance.GetNeighbors(current))
                {
                    if (!closedSet.Contains(n))
                    {
                        if (!openSet.Contains(n))
                        {
                            //Debug.Log("Added node");
                            openSet.Add(n);
                        }
                    }
                }
            }
            if (!found)
            {
                //Debug.Log("Open set: " + openSet.Count + ", Closed set: " + closedSet.Count);
                status = 2;
                return;
            }
            //Debug.Log("Found: " + i);
        }
        //Debug.Log("Touched: " + touchedNodes);
        status = 1;
    }

    /*void ResetNodes()
    {
        foreach (Node n in MapManager.mapNodes)
        {
            n.claimed = false;
        }
    }*/

    Node[,] GetNodesFromLocationV3(Vector3 pos, int size)
    {
        float l = MapManager.nodeLength;

        Node[,] nodes = new Node[size, size];

        float x = pos.x - (size / 2) * MapManager.nodeLength;
        float z = pos.z - (size / 2) * MapManager.nodeLength;

        for (int q = 0; q < size; q++)
        {
            for (int w = 0; w < size; w++)
            {
                if (nodes[q, w] == null)
                {
                    Vector3 nPos = Vector3.zero;
                    nPos.y = pos.y;
                    nPos.x = x + (l * q);
                    nPos.z = z + (w * l);
                    nodes[q, w] = MapManager.instance.GetNodeFromLocation(nPos);
                }
            }
        }
        return nodes;
    }

    bool NodesAreOK(Node[,] nodes)
    {
        foreach (Node n in nodes)
        {
            if (n == null)
            {
                Debug.Log("NULL NODE");
                return false;
            }
            //if (n.claimed || !n.walkable)
            if(claimed.Contains(n) || !n.isWalkable)
            {
                return false;
            }
        }
        return true;
    }

    Vector3 GetAvgPosition(Node[,] n)
    {
        //Debug.Log("Length: " + Mathf.Sqrt(n.Length));
        int side = Mathf.FloorToInt(Mathf.Sqrt(n.Length));
        //Debug.Log("Start: " + n[0,0].Position + ", End: " + n[side - 1, 0].Position + ", Middle: " + ((n[0, 0].Position.x + n[side - 1, 0].Position.x) / 2));
        float x = ((n[0, 0].GetPosition.x + n[side - 1, side - 1].GetPosition.x) / 2) + MapManager.nodeLength / 2;
        float z = ((n[0, 0].GetPosition.z + n[side - 1, side - 1].GetPosition.z) / 2) + MapManager.nodeLength / 2;
        return new Vector3(x - 0.01f, n[0, 0].GetPosition.y, z - 0.01f);
    }
}
