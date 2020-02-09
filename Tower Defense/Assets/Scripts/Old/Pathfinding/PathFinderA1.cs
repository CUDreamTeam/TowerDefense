using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PathFinderA1
{
    public int status = 0;

    public Thread thread = null;
    private Node start = null;
    private Node goal = null;
    //width * width = number of nodes a unit takes up
    readonly int width = 0;
    private bool pathSuccess = false;
    public List<Node> path = null;
    public List<Node> simplified = null;
    public List<Vector3> vPath = null;
    public List<Vector3> simplifiedVPath = null;

    public List<Node> critical = new List<Node>();

    int occCode = 0;

    /*public PathFinderA1V2(Node _start, Node _goal)
    {
        start = _start;
        goal = _goal;
        StartThread();
    }*/

    public PathFinderA1(PathRequest r)
    {
        start = r.start;
        goal = r.end;
        width = r.requestee.size;
        occCode = r.occCode;

        //Debug.Log("Target: " + r.end.Position);
        //MapManager.instance.printLocations.Add(r.end.Position);

        StartThread();
    }

    public void StartThread()
    {
        ResetNodes();//reset all nodes to ensure correct path has been made
        ThreadStart startT = new ThreadStart(AStar);
        thread = new Thread(startT);
        thread.Start();
    }

    private void ResetNodes()
    {
        foreach (Node n in MapManager.mapNodes)
        {
            n.ClearAStar();
        }
    }

    private void AStar()
    {
        if (start == null || goal == null)
        {
            Debug.Log("Start or Goal are null, pathfinding cant be completed");
            //failed = true;
            status = 2;
            return;
        }

        if (!start.isWalkable || !goal.isWalkable)
        {
            Debug.Log("Start or Goal are not walkable, pathfinding cant be completed");
            //failed = true;
            status = 2;
            return;
        }

        if (!NodesAreOK(GetNodesFromLocation(start.GetPosition)))
        {
            Debug.Log("Start nodes are not ok");
            status = 2;
            return;
        }
        if (!NodesAreOK(GetNodesFromLocation(goal.GetPosition)))
        {
            Debug.Log("Goal nodes are not ok");
            status = 2;
            return;
        }

        MinHeap<Node> openSet = new MinHeap<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.addItem(start);
        long viewed = 0;
        while (openSet.size > 0)
        {
            Node currentNode = openSet.getFront();
            closedSet.Add(currentNode);
            viewed++;
            if (currentNode == goal)
            {
                //Add callback
                pathSuccess = true;
                break;
            }

            foreach (Node n in MapManager.instance.GetNeighbors(currentNode))
            {
                if (!n.isWalkable || closedSet.Contains(n) || (n.occCode != -1 && n.occCode != occCode) || !NodesAreOK(GetNodesFromLocation(n.GetPosition)))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, n) + n.moveCost;
                bool temp = openSet.Contains(n);
                if (newMovementCostToNeighbour <= n.gCost || !temp)
                {
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, goal);
                    n.parent = currentNode;

                    if (!temp)
                        openSet.addItem(n);
                    else
                        openSet.UpdateItem(n);
                }
            }
        }
        if (pathSuccess)
        {
            RetracePath();
            //Debug.Log("Path succeeded");
        }
        /*else
        {
            Debug.Log("Path failed, open count: " + openSet.size + ", closed count: " + closedSet.Count);
        }*/
    }

    /*int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.xIndex - nodeB.xIndex);
        int dstZ = Mathf.Abs(nodeA.zIndex - nodeB.zIndex);

        if (dstX > dstZ)
            return (14 * dstZ) + (10 * (dstX - dstZ));
        return (14 * dstX) + (10 * (dstZ - dstX));
    }*/

    int GetDistance(Node nodeA, Node nodeB)
    {
        return Mathf.FloorToInt(Vector3.Distance(nodeA.GetPosition, nodeB.GetPosition));
    }

    private void RetracePath()
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

        /*vPath = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            vPath.Add(path[i].Position);
        }*/

        //Debug.Log("Path contains goal: " + vPath.Contains(goal.Position));

        //SimplifyPath();
        //path = simplified;
        //buildVPath();
        //done = true;
        status = 1;
    }

    List<Node> criticalNodes(List<Node> currentNodes)
    {
        List<Node> toReturn = new List<Node>();
        Vector3 oldDir = Vector2.zero;

        toReturn.Add(currentNodes[0]);
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 newDir = currentNodes[i - 1].GetPosition - currentNodes[i].GetPosition;
            if (newDir != oldDir)
            {
                toReturn.Add(currentNodes[i - 1]);
            }
            //else if (currentNodes[i].critical)
            else if(NodeIsCritical(currentNodes[i]))
            {
                toReturn.Add(currentNodes[i]);
            }
            oldDir = newDir;
        }
        //Debug.Log("Finished getting critical nodes: " + currentNodes.Count + " to " + toReturn.Count);
        return toReturn;
    }

    bool NodeIsCritical(Node n)
    {
        List<Node> temp = MapManager.instance.GetNeighbors(n);
        foreach (Node o in temp) if (!o.isWalkable) return true;
        return false;
    }

    /*List<Node> ReduceCritical(List<Node> critical)
    {
        try
        {
            for (int i = 0; i < critical.Count; i++)
            {
                for (int j = critical.Count - 1; j > i + 1; j--)
                {
                    bool b;
                    bool v;
                    try
                    {
                        Debug.Log("J = critical.Count - 1: " + (critical.Count + 1) + ", " + j);
                        v = ViableChange(critical[i], critical[j]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Critical count: " + critical.Count);
                        Debug.Log("J: " + (i + 1));
                        Debug.Log("Critical count: " + critical.Count + ", (" + i + "," + j + ")");
                        Debug.Log("Viable change failed: " + e.ToString());
                    }
                    try
                    {
                        b = (MoveCost(critical[i].Position, critical[j].Position) <= MoveCost(critical.GetRange(i, j - i)));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Move cost failed: " + e.ToString());
                    }
                    if (ViableChange(critical[i], critical[j]) && MoveCost(critical[i].Position, critical[j].Position) <= MoveCost(critical.GetRange(i, j - i)))
                    {
                        critical.RemoveRange(i + 1, j - i - 1);
                    }
                }
            }
        }
        catch
        {
            Debug.Log("Error in reduce critical");
            while (true) ;
        }
        return critical;
    }*/

    List<Node> ReduceCritical(List<Node> critical)
    {
        //Debug.Log("Starting reducing critical with " + critical.Count + " nodes");
        bool changed = false;
        while (true)
        {
            changed = false;
            for (int i = 0; i < critical.Count; i++)
            {
                for (int j = critical.Count - 1; j > i + 1; j--)
                {
                    //bool b = (MoveCost(critical[i].Position, critical[j].Position) <= MoveCost(critical.GetRange(i, j - i))); ;
                    //bool v = ViableChange(critical[i], critical[j]); ;
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
        //Debug.Log("Finished reducing critical to " + critical.Count + " nodes");
        return critical;
    }

    List<Vector3> NodeToVector(List<Node> nodes)
    {
        if (nodes.Count == 0)
        {
            nodes.Add(start);
            nodes.Add(goal);
        }


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

        Vector3 temp = toReturn[toReturn.Count - 1];
        toReturn.RemoveAt(toReturn.Count - 1);
        toReturn.Add(GetAvgPosition(GetNodesFromLocation(temp)));

        return toReturn;
    }

    private void SimplifyPath()
    {
        simplified = new List<Node>();
        Vector2 directionOld = Vector2.zero;
        simplified.Add(start);
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].xIndex - path[i].xIndex, path[i - 1].zIndex - path[i].zIndex);
            if (directionNew != directionOld)
            {
                if (!simplified.Contains(path[i - 1]))
                {
                    simplified.Add(path[i - 1]);
                    critical.Add(path[i - 1]);
                }
            }
            //if (path[i].critical)
            if(NodeIsCritical(path[i]))
            {
                if (!simplified.Contains(path[i]))
                {
                    simplified.Add(path[i]);
                    critical.Add(path[i - 1]);
                }
            }
            directionOld = directionNew;
        }
        simplified.Add(goal);
        //Debug.Log(simplified.Count);
        path = simplified;
        //return;
        bool changed = false;
        while (true)
        {
            changed = false;
            for (int i = 0; i < simplified.Count; i++)
            {
                if (changed || simplified.Count <= 2)
                {
                    break;
                }
                for (int j = simplified.Count - 1; j > i; j--)
                {
                    if (ViableChange(simplified[i], simplified[j]) && Math.Abs(j - i) != 1 && MoveCost(simplified[i].GetPosition, simplified[j].GetPosition) <= MoveCost(simplified.GetRange(i, j - i)))
                    {
                        simplified.RemoveRange(i + 1, j - i - 1);
                        changed = true;
                        break;
                    }
                }
            }
            if (!changed)
            {
                break;
            }
        }
        critical = simplified;

        bool test = true;
        foreach (Node n in simplified) if (!NodesAreOK(GetNodesFromLocation(n.GetPosition))) test = false;
        Debug.Log("All nodes are ok after critical: " + test);

        simplifiedVPath = new List<Vector3>();
        foreach (Node n in simplified)
        {
            simplifiedVPath.Add(GetAvgPosition(GetNodesFromLocation(n.GetPosition)));
        }

        test = true;
        foreach (Node n in simplified) if (!NodesAreOK(GetNodesFromLocation(n.GetPosition))) test = false;
        Debug.Log("All nodes are ok after simplified: " + test);

        vPath = new List<Vector3>();
        for (int i = 0; i < simplifiedVPath.Count - 1; i++)
        {
            Vector3 s = simplifiedVPath[i];
            Vector3 e = simplifiedVPath[i + 1];
            Vector3 last = s;
            Vector3 current = s;
            float distance = Vector3.Distance(e, current);
            Vector3 change = (e - s) * (1 / (distance / .5f));
            while (distance >= Vector3.Distance(e, current))
            {
                distance = Vector3.Distance(e, current);
                current += change;
                if (MapManager.instance.GetNodeFromLocation(last) != MapManager.instance.GetNodeFromLocation(current))
                {
                    last = current;
                    vPath.Add(current);
                }
            }
            vPath.RemoveAt(vPath.Count - 1);
        }
        if (vPath.Count != 0)
        {
            Vector3 temp = vPath[vPath.Count - 1];
            vPath.RemoveAt(vPath.Count - 1);
            vPath.Add(GetAvgPosition(GetNodesFromLocation(temp)));
        }
        test = true;
        foreach (Vector3 n in vPath) if (!NodesAreOK(GetNodesFromLocation(n))) test = false;
        Debug.Log("All nodes are ok after vPath: " + test);
        //Debug.Log("Finished A2: " + vPath.Count);
    }

    /*void buildVPath()
    {
        vPath = new List<Vector3>();
        for (int i = 0; i < simplified.Count - 1; i++)
        {
            Node s = simplified[i];
            Node e = simplified[i + 1];
            Node last = s;
            Vector3 current = s.position;
            Vector3 change = (e.position - s.position) * .01f;

            float distance = Vector3.Distance(getAvgNodePosition(getNodesFromLocation(e.position)[0,0]), getAvgNodePosition(getNodesFromLocation(current)[0,0]));
            while (distance >= Vector3.Distance(e.position, current))
            {
                distance = Vector3.Distance(getAvgNodePosition(getNodesFromLocation(e.position)[0, 0]), current);
                current += change;
                if (last != Map.instance.getNodeFromLocation(current))
                {
                    last = Map.instance.getNodeFromLocation(current);
                    vPath.Add(current);
                }
            }
            //vPath.RemoveAt(vPath.Count-1);
        }
        Vector3 temp = getAvgNodePosition(getNodesFromLocation(vPath[vPath.Count - 1])[0, 0]);
        vPath.RemoveAt(vPath.Count - 1);
        vPath.Add(temp);
    }*/

    void BuildVPath_old()
    {
        List<Vector3> newNodes = new List<Vector3>();
        vPath = new List<Vector3>();
        for (int i = 0; i < simplified.Count - 1; i++)
        {
            Node s = simplified[i];
            Node e = simplified[i + 1];
            Node last = s;
            Vector3 current = s.GetPosition;
            Vector3 change = (e.GetPosition - s.GetPosition) * .01f;

            float distance = Vector3.Distance(e.GetPosition, current);
            while (distance >= Vector3.Distance(e.GetPosition, current))
            {
                distance = Vector3.Distance(e.GetPosition, current);
                current += change;
                if (last != MapManager.instance.GetNodeFromLocation(current))
                {
                    last = MapManager.instance.GetNodeFromLocation(current);
                    vPath.Add(current);
                }
            }
            vPath.RemoveAt(vPath.Count - 1);
        }
        /*Vector3 temp = getAvgNodePosition(getNodesFromLocation(vPath[vPath.Count - 1])[0, 0]);
        vPath.RemoveAt(vPath.Count - 1);
        vPath.Add(temp);*/
    }
    void BuildVPath2()
    {
        List<Vector3> newNodes = new List<Vector3>();
        vPath = new List<Vector3>();
        for (int i = 0; i < simplified.Count - 1; i++)
        {
            Node start = simplified[i];
            Node end = simplified[i + 1];
            Vector3 current = start.GetPosition;
            float distance = Vector3.Distance(end.GetPosition, current);
            Vector3 change = (end.GetPosition - start.GetPosition) * (1 / (distance / .5f));
            List<Node> nodes = new List<Node>();
            Node last = null;

            while (distance >= Vector3.Distance(end.GetPosition, current))
            {
                distance = Vector3.Distance(end.GetPosition, current);
                current += change;
                if (last != MapManager.instance.GetNodeFromLocation(current))
                {
                    last = MapManager.instance.GetNodeFromLocation(current);
                    vPath.Add(current);
                }
            }
            vPath.RemoveAt(vPath.Count - 1);
        }
    }

    bool ViableChange(Node s, Node g)
    {
        Vector3 start = s.GetPosition;
        Vector3 end = g.GetPosition;
        Vector3 current = start;
        float distance = Vector3.Distance(end, current);
        Vector3 change = (end - start) * (1 / (distance / .5f));
        List<Node> nodes = new List<Node>();
        int count = 0;

        //Debug.Log("Start: " + start + " Goal: " + end + " change: " + change + " distance: " + distance);
        try
        {
            while (distance >= Vector3.Distance(end, current) && current != end)
            {
                count++;
                distance = Vector3.Distance(end, current);
                current += change;
                //Node toTest = MapManager.instance.GetNodeFromLocation(current);
                if (!NodesAreOK(GetNodesFromLocation(current)))
                {
                    return false;
                }

                /*if (!toTest.walkable || (toTest.GetOccCode() != occCode && toTest.GetOccCode() != -1))
                {
                    return false;
                }*/
            }
        }
        catch
        {
            Debug.Log("Viable change: total ticks: " + count + " estimated: " + (1 / (distance / .5f)));
            return false;
        }

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

    //Review, This is what is broken
    int MoveCost(Vector3 s, Vector3 g)
    {
        int cost = 0;
        int count = 0;
        Vector3 start = s;
        Vector3 end = g;
        Vector3 current = start;
        float distance = Vector3.Distance(end, current);
        Vector3 change = (end - start) * (1 / (distance / .5f));
        Node prev = null;

        try
        {
            //Debug.Log("Start: " + start + " Goal: " + end + " change: " + change + " distance: " + distance);

            while (distance >= Vector3.Distance(end, current) && current != end)
            {
                count++;
                distance = Vector3.Distance(end, current);
                current += change;
                Node toTest = MapManager.instance.GetNodeFromLocation(current);
                if (toTest != prev)
                {
                    cost += toTest.moveCost;
                    prev = toTest;
                }
            }
        }
        catch
        {
            Debug.Log("Failed to get move cost. Estimated ticks: " + (1 / (distance / .5f)) + " total ticks: " + count);
        }

        return cost;

        /*int currentCost = 0;

        Vector3 current = a;
        Vector3 change = (b - a) * MapManager.length;
        float dist = Vector3.Distance(current, b);

        while (dist >= Vector3.Distance(current, b))
        {
            currentCost += MapManager.instance.GetNodeFromLocation(current).moveCost;
            dist = Vector3.Distance(b, current);
            current += change;
        }

        return currentCost;*/
    }

    Vector3 GetAvgPosition(Node[,] n)
    {
        float x = ((n[0, 0].GetPosition.x + n[0, n.GetLength(n.Rank - 1) - 1].GetPosition.x) / 2) + MapManager.nodeLength / 2;
        float z = ((n[0, 0].GetPosition.z + n[n.GetLength(n.Rank - 1) - 1, 0].GetPosition.z) / 2) + MapManager.nodeLength / 2;
        return new Vector3(x - 0.01f, n[0, 0].GetPosition.y, z - 0.01f);
    }
    /*Node[,] GetNodesFromLocation(Vector3 pos)
    {
        float l = MapManager.length;

        Node[,] nodes = new Node[width * 2, width * 2];

        float x = pos.x - (width / 2);
        float z = pos.z - (width / 2);

        for (int q = 0; q < width * 2; q++)
        {
            for (int w = 0; w < width * 2; w++)
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
    }*/
    Node[,] GetNodesFromLocation(Vector3 pos)
    {
        float l = MapManager.nodeLength;

        Node[,] nodes = new Node[width, width];

        float x = pos.x - (width / 2) * MapManager.nodeLength;
        float z = pos.z - (width / 2) * MapManager.nodeLength;

        for (int q = 0; q < width; q++)
        {
            for (int w = 0; w < width; w++)
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
            /*if(n == null)
            {
                badNode = true;
                return false;
            }*/
            if (n == null)
            {
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
}
