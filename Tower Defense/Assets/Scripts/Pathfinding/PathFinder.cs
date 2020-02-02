using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class PathFinder
{
    //0-InProgress
    //1-Succeeded
    //2-Failed
    public int status = 0;

    public Thread threadA = null;
    public Thread threadB = null;
    private Node start = null;
    private Node goal = null;

    //Units take up a square section of nodes
    private int unitWidth = 0;

    public List<Vector3> finalPath = null;

    //Used for efficiency, reduce the max search size
    private Vector2Int max = Vector2Int.zero;
    private Vector2Int min = Vector2Int.zero;

    //What node did thread A or thread b exit on
    private Node startExit = null;
    private Node goalExit = null;

    private bool succeeded = false;

    public PathFinder(PathRequest r)
    {
        start = r.start;
        goal = r.end;
        unitWidth = r.requestee.size;

        //Build max bounds
        max = new Vector2Int((start.xIndex < goal.xIndex) ? goal.xIndex : start.xIndex,
                              (start.zIndex < goal.zIndex) ? goal.zIndex : start.zIndex);
        max.x = (max.x + 10 < MapManager.instance.xNodes) ? max.x + 10 : MapManager.instance.xNodes - 1;
        max.y = (max.y + 10 < MapManager.instance.zNodes) ? max.y + 10 : MapManager.instance.zNodes - 1;

        min = new Vector2Int((start.xIndex > goal.xIndex) ? goal.xIndex : start.xIndex,
                             (start.zIndex > goal.zIndex) ? goal.zIndex : start.zIndex);
        min.x = (min.x - 10 >= 0) ? min.x - 10 : 0;
        min.y = (min.y - 10 >= 0) ? min.y - 10 : 0;

        ResetNodes();

        if (start == null || goal == null)
        {
            Debug.Log("Failed to start pathfinder, start or goal is null");
            status = 2;
            return;
        }

        if (!start.isWalkable || !goal.isWalkable)
        {
            Debug.Log("Failed to start pathfinder, start or goal is unwalkable");
            status = 2;
            return;
        }

        threadA = new Thread(new ThreadStart(AStarStart));
        threadB = new Thread(new ThreadStart(AStarGoal));
        threadA.Start();
        threadB.Start();
    }

    private void ResetNodes()
    {
        foreach (Node n in MapManager.mapNodes) n.ClearAStar();
    }

    private void AStarStart()
    {
        MinHeap<Node> openSet = new MinHeap<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.addItem(start);

        while (openSet.size > 0 && !succeeded)
        {
            Node currentNode = openSet.getFront();
            //MapManager.instance.testLocations.Add(currentNode.Position);
            closedSet.Add(currentNode);
            //If touched by other thread
            /*if (currentNode.touched == 2)
            {
                startExit = currentNode;
                succeeded = true;
                //Path success
                RetracePath();
            }*/

            foreach (Node n in MapManager.instance.GetNeighbors(currentNode))
            {
                if (n.touched == 2)
                {
                    startExit = currentNode;
                    goalExit = n;
                    succeeded = true;
                    RetracePath();
                    return;
                }
                else if (n == goal)
                {
                    //Debug.Log("From start found goal");
                    RetracePathStart();
                    return;
                }

                if (!n.isWalkable || closedSet.Contains(n) || !InRange(n) || n.touched != 0)
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, n) + n.moveCost;
                bool inOpenSet = openSet.Contains(n);

                if (!inOpenSet)
                {
                    n.touched = 1;
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, goal);
                    n.parent = currentNode;
                    openSet.addItem(n);
                }
                else if (newMovementCostToNeighbour < n.gCost)
                {
                    n.touched = 1;
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, goal);
                    n.parent = currentNode;
                    openSet.UpdateItem(n);
                }

                /*if (newMovementCostToNeighbour <= n.gCost || !inOpenSet)
                {
                    n.touched = 1;
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, goal);
                    n.parent = currentNode;

                    if (!inOpenSet)
                        openSet.addItem(n);
                    else
                        openSet.UpdateItem(n);
                }*/
            }
        }
    }

    void RetracePathStart()
    {
        List<Node> rebuilt = new List<Node>();
        Node currentNode = start;

        while (currentNode.parent != null)
        {
            rebuilt.Add(currentNode);
            currentNode = currentNode.parent;
        }

        finalPath = NodeToVector(ReduceCritical(criticalNodes(rebuilt)));
        status = 1;
    }

    private void AStarGoal()
    {
        //        Debug.Log("Started search from goal: " + goal.Position);

        MinHeap<Node> openSet = new MinHeap<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.addItem(goal);

        while (openSet.size > 0 && !succeeded)
        {
            Node currentNode = openSet.getFront();
            //MapManager.instance.testLocations2.Add(currentNode.Position);
            closedSet.Add(currentNode);
            /*if (currentNode.touched == 1)
            {
                goalExit = currentNode;
                succeeded = true;
                //Path success
                if (startExit != null && goalExit != null) RetracePath();
                else
                {
                    Debug.Log("Goal finished first");
                    return;
                }
            }*/

            foreach (Node n in MapManager.instance.GetNeighbors(currentNode))
            {
                if (n.touched == 1)
                {
                    goalExit = currentNode;
                    startExit = n;
                    succeeded = true;
                    RetracePath();
                    return;
                }
                else if (n == start)
                {
                    //Debug.Log("From goal found start");
                    RetracePathGoal();
                    return;
                }

                if (!n.isWalkable || closedSet.Contains(n) || !InRange(n) || n.touched != 0)
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, n) + n.moveCost;
                bool inOpenSet = openSet.Contains(n);
                if (!inOpenSet)
                {
                    n.touched = 2;
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, start);
                    n.parent = currentNode;
                    openSet.addItem(n);
                }
                else if (newMovementCostToNeighbour < n.gCost)
                {
                    n.touched = 2;
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, start);
                    n.parent = currentNode;
                    openSet.UpdateItem(n);
                }

                /*if (newMovementCostToNeighbour <= n.gCost || !inOpenSet)
                {
                    n.touched = 2;
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, start);
                    n.parent = currentNode;

                    if (!inOpenSet) openSet.addItem(n);
                    else openSet.UpdateItem(n);
                }*/
            }
        }
        //        Debug.Log("Exited goal loop: " + succeeded);
    }

    void RetracePathGoal()
    {
        List<Node> rebuilt = new List<Node>();
        Node currentNode = goal;

        while (currentNode.parent != null)
        {
            rebuilt.Add(currentNode);
            currentNode = currentNode.parent;
        }

        finalPath = NodeToVector(ReduceCritical(criticalNodes(rebuilt)));
        status = 1;
    }

    bool InRange(Node n)
    {
        //Debug.Log("In range: " + (n.xIndex <= max.x && n.xIndex >= min.x && n.zIndex <= max.y && n.zIndex >= min.y));
        return (n.xIndex <= max.x && n.xIndex >= min.x && n.zIndex <= max.y && n.zIndex >= min.y);
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        return Mathf.FloorToInt(Vector3.Distance(nodeA.GetPosition, nodeB.GetPosition));
    }

    /*private void RetracePath()
    {
        path = new List<Node>();
        Node currentNode = goal;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        vPath = NodeToVector(ReduceCritical(criticalNodes(path)));
        status = 1;
    }*/

    private void RetracePath()
    {
        List<Node> fromStart = new List<Node>();
        List<Node> fromEnd = new List<Node>();

        List<Vector3> group1 = new List<Vector3>();
        List<Vector3> group2 = new List<Vector3>();

        Node currentNode = startExit;
        while (currentNode != null && currentNode != goal)
        {
            group1.Add(currentNode.GetPosition);
            fromStart.Add(currentNode);
            currentNode = currentNode.parent;
        }

        currentNode = goalExit;
        while (currentNode != null && currentNode != start)
        {
            group2.Add(currentNode.GetPosition);
            fromEnd.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if (MapManager.instance.GetNeighbors(fromStart[0]).Contains(fromEnd[0]))
        {
            fromStart.Reverse();
            fromStart.AddRange(fromEnd);
            finalPath = NodeToVector(ReduceCritical(criticalNodes(fromStart)));
            status = 1;
        }
        else
        {
            if (fromStart.Contains(goal) && fromStart.Contains(start))
            {
                Debug.Log("From start contains both start and goal");
                RetracePathStart();
            }
            if (fromEnd.Contains(goal) && fromEnd.Contains(start))
            {
                Debug.Log("From end contains both start and goal");
                RetracePathGoal();
            }

            /*status = 2;
            foreach (Node n in fromStart)
            {
                MapManager.instance.testLocations.Add(n.Position);
            }
            foreach (Node n in fromEnd)
            {
                MapManager.instance.testLocations.Add(n.Position);
            }
            Debug.Log("Pathfinding succceeded but failed");*/
        }
        ResetNodes();
        threadA.Abort();
        threadB.Abort();
    }

    List<Vector3> NodeToVector(List<Node> nodes)
    {
        List<Vector3> toReturn = new List<Vector3>();
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            Vector3 start = nodes[i].GetPosition;
            Vector3 end = nodes[i + 1].GetPosition;
            Vector3 current = nodes[i].GetPosition;
            float distance = Vector3.Distance(start, end);
            Vector3 change = (end - start) * (1 / (distance / .5f));
            while (distance >= Vector3.Distance(end, current))
            {
                distance = Vector3.Distance(end, current);
                toReturn.Add(current);
                current += change;
            }
        }
        return toReturn;
    }

    List<Node> ReduceCritical(List<Node> critical)
    {
        try
        {
            bool changed = false;
            while (true)
            {
                changed = false;
                for (int i = 0; i < critical.Count; i++)
                {
                    for (int j = critical.Count - 1; j > i + 1; j--)
                    {
                        if (ViableChange(critical[i], critical[j]) && MoveCost(critical[i].GetPosition, critical[j].GetPosition) <= MoveCost(critical.GetRange(i, j - i)))
                        {
                            critical.RemoveRange(i + 1, j - i - 1);
                            changed = true;
                            break;
                        }
                    }
                    if (changed) break;
                }
                if (!changed) break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Reduce critical failed: " + e.ToString());
        }
        return critical;
    }

    List<Node> criticalNodes(List<Node> currentNodes)
    {
        List<Node> toReturn = new List<Node>();
        Vector3 oldDir = Vector2.zero;

        if (currentNodes.Count == 0)
        {
            //Debug.Log("FML, critical nodes got a list with nothing in it");
            currentNodes.Add(start);
            currentNodes.Add(goal);
        }

        toReturn.Add(currentNodes[0]);
        for (int i = 1; i < currentNodes.Count; i++)
        {
            Vector3 newDir = currentNodes[i - 1].GetPosition - currentNodes[i].GetPosition;
            if (newDir != oldDir)
            {
                toReturn.Add(currentNodes[i - 1]);
            }
            //else if (currentNodes[i].critical)
            else if (NodeIsCritical(currentNodes[i])) 
            {
                toReturn.Add(currentNodes[i]);
            }
            oldDir = newDir;
        }
        if (!toReturn.Contains(currentNodes[currentNodes.Count - 1])) toReturn.Add(currentNodes[currentNodes.Count - 1]);

        return toReturn;
    }

    bool NodeIsCritical(Node n)
    {
        List<Node> temp = MapManager.instance.GetNeighbors(n);
        foreach (Node o in temp) if(!o.isWalkable) return true;
        return false;
    }

    bool ViableChange(Node s, Node g)
    {
        //Debug.Log("Checking if viable change");
        Vector3 start = s.GetPosition;
        Vector3 end = g.GetPosition;
        Vector3 current = start;
        float distance = Vector3.Distance(end, current);
        Vector3 change = (end - start) * (1 / (distance / .5f));
        List<Node> nodes = new List<Node>();

        while (distance >= Vector3.Distance(end, current) && current != end)
        {
            distance = Vector3.Distance(end, current);
            current += change;
            if (!MapManager.instance.GetNodeFromLocation(current).isWalkable)
            {
                return false;
            }
        }
        //Debug.Log("Viable change");
        return true;
    }

    int MoveCost(List<Node> n)
    {
        int t = 0;
        for (int i = 0; i < n.Count - 1; i++)
        {
            t += MoveCost(n[i].GetPosition, n[i + 1].GetPosition);
        }
        return t;
    }

    int MoveCost(Vector3 a, Vector3 b)
    {
        int cost = 0;
        Vector3 start = a;
        Vector3 end = b;
        Vector3 current = start;
        float distance = Vector3.Distance(end, current);
        Vector3 change = (end - start) * (1 / (distance / .5f));
        Node prev = null;

        while (distance >= Vector3.Distance(end, current) && current != end)
        {
            distance = Vector3.Distance(end, current);
            current += change;
            Node toTest = MapManager.instance.GetNodeFromLocation(current);
            if (toTest != prev)
            {
                cost += toTest.moveCost;
                prev = toTest;
            }
        }
        return cost;
    }

    Vector3 GetAvgPosition(Node[,] n)
    {
        float x = ((n[0, 0].GetPosition.x + n[0, n.GetLength(n.Rank - 1) - 1].GetPosition.x) / 2) + MapManager.nodeLength / 2;
        float z = ((n[0, 0].GetPosition.z + n[n.GetLength(n.Rank - 1) - 1, 0].GetPosition.z) / 2) + MapManager.nodeLength / 2;
        return new Vector3(x - 0.01f, n[0, 0].GetPosition.y, z - 0.01f);
    }
    Node[,] GetNodesFromLocation(Vector3 pos)
    {
        float l = MapManager.nodeLength;

        Node[,] nodes = new Node[unitWidth * 2, unitWidth * 2];

        float x = pos.x - (unitWidth / 2);
        float z = pos.z - (unitWidth / 2);

        for (int q = 0; q < unitWidth * 2; q++)
        {
            for (int w = 0; w < unitWidth * 2; w++)
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
}
