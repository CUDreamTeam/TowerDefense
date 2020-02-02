using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//Player units should always have a team code of 0
public class UnitSelection : MonoBehaviour
{
    public static UnitSelection instance = null;

    public List<UnitBase> playerUnits = new List<UnitBase>();
    public List<UnitBase> selected = new List<UnitBase>();
    bool isSelecting = false;

    Vector3 mousePos;
    Vector3 mousePos2;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
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
            selected = new List<UnitBase>();
        }

        if (Input.GetMouseButtonUp(1))
        {
            foreach (UnitBase u in playerUnits)
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
            foreach (UnitBase u in playerUnits)
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
                //print("Started search, " + selected.Count);
                //sendMoveLoc(hit.point);
                //MapManager.instance.AddPathRequest(hit.point, selected, 0);
                Thread temp = new Thread(unused => BuildMoveLocations(selected, hit.point));
                temp.Start();
            }
        }
    }

    void BuildMoveLocations(List<UnitBase> units, Vector3 start)
    {
        PlacementSearch temp = new PlacementSearch(units, MapManager.instance.GetNodeFromLocation(start));
        while (temp.status == 0) ;
        for (int i = 0; i < units.Count; i++)
        {
            //MapManager.instance.AddPathRequest(temp.movePos[i], units[i], 0);
            units[i].RequestPath(temp.movePos[i], 1, 0);
        }
    }

    void SendMoveLoc(Vector3 pos)
    {
        if (selected.Count == 0)
        {
            return;
        }

        MinHeap<UnitBase> ordered = new MinHeap<UnitBase>(selected);
        UnitBase[,] placed = null;
        int layers = estimateLayers();

        placed = new UnitBase[layers, layers];

        for (int i = 0; i < layers; i++)
        {
            for (int j = 0; j < layers; j++)
            {
                if (ordered.size > 0)
                    placed[i, j] = ordered.getFront();
            }
        }

        Vector3 position = pos;

        position.x -= placed[0, 0].size * (layers / 2);
        position.z -= placed[0, 0].size * (layers / 2);

        float x = position.x;
        float increase = 0;

        for (int i = 0; i < layers; i++)
        {
            position.x = x;
            for (int j = 0; j < layers; j++)
            {
                if (placed[i, j] != null)
                {
                    placed[i, j].RequestPath(position, 0, 0);
                    position.x += placed[i, j].size * 2;
                    increase = placed[i, j].size;
                }
            }
            position.z += increase * 2;
        }
    }

    int estimateLayers()
    {
        int space = 0;
        int layers = 0;

        foreach (UnitBase u in selected)
        {
            space += u.size * u.size;
        }

        while (layers * layers < space)
        {
            layers++;
        }

        return layers;
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

    private void OnApplicationQuit()
    {
        selected = null;
        foreach (UnitBase u in playerUnits)
        {
            Destroy(u);
        }
        playerUnits = null;
    }
}
