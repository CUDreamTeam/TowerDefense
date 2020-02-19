using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    private int collisions = 0;

    private Renderer rend = null;

    public int teamCode = 0;
    [SerializeField] private AttackableObject toSpawn = null;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void Populate(int TeamCode)
    {
        teamCode = TeamCode;
    }

    public bool CanPlace()
    {
        return collisions == 0;
    }

    private void Update()
    {
        if (collisions == 0)
        {
            rend.material.color = Color.green;
        }
        else
        {
            rend.material.color = Color.red;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground")
        {
            collisions++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Ground")
        {
            collisions--;
        }
    }

    public bool TryPlace()
    {
        if (collisions == 0)
        {
            GameObject g = Instantiate(toSpawn.gameObject, transform.position, transform.rotation);
            g.GetComponent<AttackableObject>().Populate(teamCode);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
