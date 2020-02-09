using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class UnitSearch
{
    //Search range
    readonly int range = 0;
    //Player code will be used to identify teams
    readonly int teamCode = 0;
    readonly int unitCode = 0;

    public enum SearchStatus { inProcess, succeeded, failed };
    public SearchStatus status = SearchStatus.inProcess;

    private Node start = null;
    private Thread thread = null;

    public UnitBase result = null;

    public UnitSearch(int _range, int uCode, int _teamCode, Node _start)
    {
        start = _start;
        range = _range;
        teamCode = _teamCode;
        unitCode = uCode;

        ThreadStart startT = new ThreadStart(Search);
        thread = new Thread(startT);
        thread.Start();
    }

    private void Search()
    {
        status = SearchStatus.inProcess;
        //Nodes to search
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

//        Debug.Log("Starting search");

        openSet.Add(start);
        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(current);

            //If current node is populated and the populated unit does is not the searcher
            if (current.occCode != -1 && current.occCode != unitCode)
            {
                UnitBase u = UnitManager.instance.GetUnitFromCode(current.occCode);
//                Debug.Log("Found unit");
                //Check if populated unit is not on the same team
                if (u.teamCode != teamCode)
                {
                    result = UnitManager.instance.GetUnitFromCode(current.occCode);
                    status = SearchStatus.succeeded;
                    return;
                }
            }

            /*UnitBase u = UnitManager.instance.GetUnitFromCode(current.occCode);
            if (current.occCode != -1 && u != null && u.teamCode != teamCode)
            {
                //Debug.Log("Unit search succeeded");
                result = UnitManager.instance.GetUnitFromCode(current.occCode);
                status = SearchStatus.succeeded;
                return;
            }*/

            //Add neighbors to search if a unit wasn't found
            foreach (Node n in MapManager.instance.GetNeighbors(current))
            {
                if (n != null && !closedSet.Contains(n) && Vector3.Distance(start.GetPosition, n.GetPosition) <= range)
                {
                    if (!openSet.Contains(n))
                    {
                        openSet.Add(n);
                    }
                }
            }
        }
        status = SearchStatus.failed;
    }
}
