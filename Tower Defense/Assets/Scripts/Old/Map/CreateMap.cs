using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap
{
    public bool GetMapIsReady
    {
        get
        {
            return mapIsReady;
        }
    }
    private bool mapIsReady = false;

    //Values used when creating map
    private float nodeLength = 0.5f;
    private float xSize = 0;
    private float zSize = 0;
    public Node[,] mapNodes = null;

    public CreateMap(float length, float _xSize, float _zSize)
    {
        nodeLength = length;
        xSize = _xSize;
        zSize = _zSize;
        BuildMap();
    }

    private void BuildMap()
    {
        //Used for setting node position in array
        int xIndex = 0;
        int zIndex = 0;

        //Gets number of node per length of 1
        int nodesPerUnit = Mathf.FloorToInt(1 / nodeLength);
        mapNodes = new Node[Mathf.FloorToInt(xSize * (1 / nodeLength)) + 1, Mathf.FloorToInt(zSize * (1 / nodeLength)) + 1];

        //Debug.Log(xSize + " : " + zSize);
        //Debug.Log((xSize * (1 / nodeLength)) + ", " + (zSize * (1 / nodeLength)));

        //Create map
        for (float x = 0; x <= xSize; x += nodeLength)
        {
            for (float z = 0; z <= zSize; z += nodeLength)
            {
                //Debug.Log(xIndex + ", " + zIndex + " : " + z);
                //Node values
                bool isWalkable = false;
                Vector3 nodePosition = Vector3.zero;
                int moveCost = 0;
                bool hit = true;

                for (float y = 5; y >= -1; y -= nodeLength)
                {
                    nodePosition = new Vector3(x, y, z);
                    Collider[] hits = Physics.OverlapSphere(nodePosition, nodeLength + 0.1f);
                    if (hits.Length > 0)
                    {
                        hit = true;
                        nodePosition = new Vector3(x, y + nodeLength, z);
                        foreach (Collider c in hits)
                        {
                            if (c.gameObject.tag == "Ground")
                            {
                                //Debug.Log("Hit ground");
                                isWalkable = true;
                                moveCost = (10 > moveCost) ? 10 : moveCost;
                            }
                            else if (c.gameObject.tag == "Road")
                            {
                                isWalkable = true;
                                moveCost = (1 > moveCost) ? 1 : moveCost;
                                //No move cost
                            }
                            else if (c.gameObject.tag == "Mud")
                            {
                                isWalkable = true;
                                moveCost = (15 > moveCost) ? 15 : moveCost;
                            }
                            else if (c.gameObject.tag == "Obstacle")
                            {
                                isWalkable = false;
                                break;
                            }
                            else
                            {
                                moveCost = (moveCost == 0) ? 1 : moveCost;
                            }
                        }
                        mapNodes[xIndex, zIndex] = new Node(isWalkable, xIndex, nodePosition.y, zIndex, moveCost);
                    }
                }
                if (!hit)
                {
                    nodePosition.y = 0;
                    //mapNodes[xIndex, zIndex] = new Node(false, new Vector3(x, 0, z), 0);
                    mapNodes[xIndex, zIndex] = new Node(false, xIndex, 0, zIndex, 0);
                }
                zIndex++;
            }
            zIndex = 0;
            xIndex++;
        }
        mapIsReady = true;
    }
}
