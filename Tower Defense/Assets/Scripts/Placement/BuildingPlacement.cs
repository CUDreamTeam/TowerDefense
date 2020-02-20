using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    public static BuildingPlacement instance = null;

    public BuildingGhost ghost = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (ghost != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 256))
            {
                Vector3 temp = hit.point;
                temp.y += ghost.transform.localScale.y / 2;
                ghost.transform.position = temp;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (ghost.TryPlace())
                {
                    ghost = null;
                }
            }
        }
    }
}
