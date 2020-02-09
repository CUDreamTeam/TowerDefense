using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IComparable<Node>
{
    //A start values
    public bool isWalkable = false;
    public int xIndex = 0;
    public int zIndex = 0;
    public int moveCost = 0;

    //A star calculation variables
    public int gCost = 0;
    public int hCost = 0;
    public Node parent = null;

    //For multithreading
    public int touched = 0;

    //Movement values
    public Vector3 GetPosition
    {
        get
        {
            return new Vector3(((float)xIndex) * MapManager.nodeLength, 0, ((float)zIndex) * MapManager.nodeLength);
        }
    }
    public int occCode = -1;
    public float y = 0;

    public Node(bool _isWalkable, int xInd, float height, int zInd, int _moveCost)
    {
        isWalkable = _isWalkable;
        xIndex = xInd;
        zIndex = zInd;
        y = height;
        moveCost = _moveCost;
    }

    public void ClearAStar()
    {
        parent = null;
        gCost = 0;
        hCost = 0;
        touched = 0;
    }

    // AStar methods
    //Full cost
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
