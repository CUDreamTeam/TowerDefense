using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//Player units should always have a team code of 0
public class UnitSelection : MonoBehaviour
{
    public static UnitSelection instance = null;

    public List<AttackableObject> playerUnits = null;
    public List<AttackableObject> selected = new List<AttackableObject>();
    bool isSelecting = false;

    Vector3 mousePos;
    Vector3 mousePos2;

    private List<Vector3> waypoints = null;

    private void Start()
    {
        instance = this;
        StartCoroutine(GetPlayerUnits());
    }

    private void Update()
    {
        if (playerUnits == null) return;
        mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(1))
        {
            mousePos2 = Input.mousePosition;

            for (int i = 0; i < selected.Count; i++)
            {
                if (selected[i] == null)
                {
                    selected.RemoveAt(i);
                    i = selected.Count;
                    continue;
                }
                selected[i].SetSelected(false);
            }
            isSelecting = true;
            selected = new List<AttackableObject>();
        }

        if (Input.GetMouseButtonUp(1))
        {
            foreach (AttackableObject u in playerUnits)
            {
                if (u == null) continue;
                if (inBounds(u.gameObject))
                {
                    selected.Add(u);
                    u.isSelected = true;
                }
            }
            isSelecting = false;
        }

        if (isSelecting)
        {
            foreach (AttackableObject u in playerUnits)
            {
                if (u == null) continue;
                if (inBounds(u.gameObject))
                {
                    u.SetSelected(true);
                }
                else
                {
                    u.SetSelected(false);
                }
            }
        }

        if (Input.GetMouseButtonDown(2) && selected.Count > 0)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (waypoints == null) waypoints = new List<Vector3>();
                    waypoints.Add(hit.point);
                }
                else
                {
                    if (waypoints == null) BuildMoveLocs(hit.point);
                    else
                    {
                        waypoints.Add(hit.point);
                        BuildMoveLocs(waypoints);
                    } 
                }
            }
        }
    }

    public int testCount = 5;
    private void BuildMoveLocs(Vector3 point)
    {
        int placed = 0;
        int toPlace = 0;
        int direction = 0;
        //1 - ->
        //2 - down
        //3 - <-
        //4 - ^
        for (int i = 0; i < selected.Count; i++)
        {
            if (direction == 0)
            {
                selected[i].OverrideMovement(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.green;
                //g.transform.position = point;
                direction = 1;
                toPlace = 1;
                placed = 0;
            }
            //Right
            else if (direction == 1)
            {
                point.x += 2;
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.blue;
                //g.transform.position = point;
                selected[i].OverrideMovement(point);
                placed++;
                if (placed == toPlace)
                {
                    placed = 0;
                    direction = 2;
                }
            }
            //Down
            else if (direction == 2)
            {
                //if (placed == 0) toPlace++;
                point.z -= 2;
                selected[i].OverrideMovement(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.red;
                //g.transform.position = point;
                placed++;
                if (placed == toPlace)
                {
                    //Debug.Log("Switching dir at " + i);
                    toPlace++;
                    placed = 0;
                    direction = 3;
                }
            }
            //Left
            else if (direction == 3)
            {
                //if (placed == 0) toPlace++;
                point.x -= 2;
                selected[i].OverrideMovement(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.yellow;
                //g.transform.position = point;
                placed++;
                if (toPlace == placed)
                {
                    //Debug.Log("Switching dir at " + i);
                    placed = 0;
                    direction = 4;
                    //if (toPlace == 2) toPlace++;
                }
            }
            //Up
            else
            {
                point.z += 2;
                selected[i].OverrideMovement(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.magenta;
                //g.transform.position = point;
                placed++;
                if (toPlace == placed)
                {
                    //Debug.Log("Switching dir at " + i);
                    placed = 0;
                    direction = 1;
                    toPlace++;
                }
            }
        }
    }

    private void BuildMoveLocs(List<Vector3> moveLocs)
    {
        List<Vector3>[] paths = new List<Vector3>[selected.Count];
        for (int i = 0; i < moveLocs.Count; i++)
        {
            List<Vector3> builtPoints = BuildPointsAtWaypoint(moveLocs[i]);
            if (i == 0)
            {
                for (int j = 0; j < selected.Count; j++) paths[j] = new List<Vector3>();
            }
            for (int j = 0; j < selected.Count; j++)
            {
                paths[j].Add(builtPoints[j]);
            }
        }
        for (int i = 0; i < selected.Count; i++)
        {
            selected[i].OverrideMovement(paths[i]);
        }
    }

    private List<Vector3> BuildPointsAtWaypoint(Vector3 point)
    {
        List<Vector3> toReturn = new List<Vector3>();
        int placed = 0;
        int toPlace = 0;
        int direction = 0;
        //1 - ->
        //2 - down
        //3 - <-
        //4 - ^
        for (int i = 0; i < selected.Count; i++)
        {
            if (direction == 0)
            {
                //selected[i].OverrideMovement(point);
                toReturn.Add(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.green;
                //g.transform.position = point;
                direction = 1;
                toPlace = 1;
                placed = 0;
            }
            //Right
            else if (direction == 1)
            {
                point.x += 2;
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.blue;
                //g.transform.position = point;
                //selected[i].OverrideMovement(point);
                toReturn.Add(point);
                placed++;
                if (placed == toPlace)
                {
                    placed = 0;
                    direction = 2;
                }
            }
            //Down
            else if (direction == 2)
            {
                //if (placed == 0) toPlace++;
                point.z -= 2;
                //selected[i].OverrideMovement(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.red;
                //g.transform.position = point;
                toReturn.Add(point);
                placed++;
                if (placed == toPlace)
                {
                    //Debug.Log("Switching dir at " + i);
                    toPlace++;
                    placed = 0;
                    direction = 3;
                }
            }
            //Left
            else if (direction == 3)
            {
                //if (placed == 0) toPlace++;
                point.x -= 2;
                //selected[i].OverrideMovement(point);
                toReturn.Add(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.yellow;
                //g.transform.position = point;
                placed++;
                if (toPlace == placed)
                {
                    //Debug.Log("Switching dir at " + i);
                    placed = 0;
                    direction = 4;
                    //if (toPlace == 2) toPlace++;
                }
            }
            //Up
            else
            {
                point.z += 2;
                //selected[i].OverrideMovement(point);
                toReturn.Add(point);
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.GetComponent<Renderer>().material.color = Color.magenta;
                //g.transform.position = point;
                placed++;
                if (toPlace == placed)
                {
                    //Debug.Log("Switching dir at " + i);
                    placed = 0;
                    direction = 1;
                    toPlace++;
                }
            }
        }
        return toReturn;
    }

    bool inBounds(GameObject g)
    {
        Camera camera = Camera.main;

        Bounds viewportBounds = Utils.GetViewportBounds(camera, mousePos, mousePos2);
        return viewportBounds.Contains(camera.WorldToViewportPoint(g.transform.position));
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            var rect = Utils.GetScreenRect(mousePos2, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    IEnumerator GetPlayerUnits()
    {
        yield return new WaitForSeconds(1);
        playerUnits = CombatHandler.instance.units[0];
    }
}
