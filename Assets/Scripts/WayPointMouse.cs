using UnityEngine;
using System.Collections;

public class WayPointMouse : MonoBehaviour
{
    public GameObject waypointPrefab;

    Waypoint prevWaypoint;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {

                var waypoint = Instantiate(waypointPrefab).GetComponent<Waypoint>();
                waypoint.transform.position = hit.point;

                if (prevWaypoint)
                {
                    prevWaypoint.nextWaypoint = waypoint.transform;
                }

                foreach (var actor in FindObjectsOfType<Actor>())
                {
                    if (!actor.waypoint)
                    {
                        actor.waypoint = waypoint.transform;
                    }

                }

                prevWaypoint = waypoint;
            }

        }
    }
}