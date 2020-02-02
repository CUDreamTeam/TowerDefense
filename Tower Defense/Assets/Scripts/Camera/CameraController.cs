using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 0.1f;
    public float rotationSpeed = 4f;
    public float smoothness = 0.85f;

    public Quaternion targetRotation;
    Vector3 targetPosition;
    float targetRotationX;
    float targetRotationY;

    private void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        targetRotationY = transform.localRotation.eulerAngles.y;
        targetRotationX = transform.localRotation.eulerAngles.x;
    }

    private void Update()
    {
        HandleMovement();
        HandleSelect();
    }

    void HandleSelect()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit something");
                SystemTile temp = hit.transform.GetComponent<SystemTile>();
                if (temp != null) {
                    SystemMapManager.instance.UpdateUI(temp);
                    temp.SetAnimation(true);
                    selectedTile?.SetAnimation(false);
                    selectedTile = temp;
                }
            }
            else
            {
                selectedTile?.SetAnimation(false);
                selectedTile = null;
                SystemMapManager.instance.UpdateUI(null);
            }
        }*/
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.Q))
            targetPosition += transform.forward * movementSpeed;
        if (Input.GetKey(KeyCode.A))
            targetPosition -= transform.right * movementSpeed;
        if (Input.GetKey(KeyCode.E))
            targetPosition -= transform.forward * movementSpeed;
        if (Input.GetKey(KeyCode.D))
            targetPosition += transform.right * movementSpeed;
        if (Input.GetKey(KeyCode.S))
            targetPosition -= transform.up * movementSpeed;
        if (Input.GetKey(KeyCode.W))
            targetPosition += transform.up * movementSpeed;

        if (Input.GetMouseButton(2))
        {
            Cursor.visible = false;
            targetRotationY += Input.GetAxis("Mouse X") * rotationSpeed;
            targetRotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
            targetRotation = Quaternion.Euler(targetRotationX, targetRotationY, 0.0f);
        }
        else
            Cursor.visible = true;

        transform.position = Vector3.Lerp(transform.position, targetPosition, (1.0f - smoothness));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (1.0f - smoothness));
    }
}
