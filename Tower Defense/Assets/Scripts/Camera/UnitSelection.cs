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

    private void Start()
    {
        instance = this;
        playerUnits = CombatHandler.instance.units[0];
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
                
            }
        }
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
