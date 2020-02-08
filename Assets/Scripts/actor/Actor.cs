using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public float moveSpeed = 2;

    public Transform waypoint;
    Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
          body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoint && body.velocity.sqrMagnitude < moveSpeed * moveSpeed)
        {
            body.AddForce((waypoint.position - transform.position).normalized * moveSpeed * 10, ForceMode.Acceleration);
        }  
    }
}
