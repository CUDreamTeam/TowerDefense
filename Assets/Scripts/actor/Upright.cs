using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upright : MonoBehaviour
{
    Rigidbody body;

    public float amount;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        body.AddForceAtPosition(Vector3.up * amount, transform.up * 10);
        body.AddForceAtPosition(Vector3.up * -amount, transform.up * -10);
    }
}
